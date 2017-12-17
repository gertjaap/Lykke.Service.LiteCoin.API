using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MoreLinq;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.TransactionOutput.SpentOutputs
{
    public class SpentOutputTableEntity : TableEntity, ISpentOutput
    {
        public string TransactionHash { get; set; }

        public int N { get; set; }
        public DateTime InsertedAt { get; set; }

        public static class ByTransactionHash
        {
            public static string GeneratePartitionKey()
            {
                return "ByTransactionHash";
            }

            public static string GenerateRowKey(string transactionHash, int n)
            {
                return $"{transactionHash}_{n}";
            }

            public static SpentOutputTableEntity Create(ISpentOutput output)
            {
                return Map(output,
                    GeneratePartitionKey(),
                    GenerateRowKey(output.TransactionHash, output.N));
            }
        }

        public static class ByDateTime
        {
            public static string GeneratePartition()
            {
                return "ByDateTime";
            }

            public static string GenerateRowKey(DateTime inserted)
            {
                return (int.MaxValue - inserted.Ticks).ToString("D");
            }

            public static SpentOutputTableEntity Create(ISpentOutput output)
            {
                return Map(output, GeneratePartition(),
                    GenerateRowKey(output.InsertedAt));
            }
        }

        private static SpentOutputTableEntity Map(ISpentOutput source, string partitionKey,
            string rowKey)
        {
            return new SpentOutputTableEntity
            {
                RowKey = rowKey,
                PartitionKey = partitionKey,
                TransactionHash = source.TransactionHash,
                N = source.N,
                InsertedAt = source.InsertedAt
            };
        }
    }

    public class SpentOutputRepository:ISpentOutputRepository
    {
        private readonly INoSQLTableStorage<SpentOutputTableEntity> _storage;
        private const int EntityExistsHttpStatusCode = 409;

        public SpentOutputRepository(INoSQLTableStorage<SpentOutputTableEntity> storage)
        {
            _storage = storage;
        }

        public async Task InsertSpentOutputs(IEnumerable<ISpentOutput> outputs)
        {
            var spentOutputs = outputs as ISpentOutput[] ?? outputs.ToArray();

            async Task ThrowIfBackend(StorageException exception)
            {
                if (exception != null && exception.RequestInformation.HttpStatusCode == EntityExistsHttpStatusCode)
                {
                    await DeleteOutputs(spentOutputs);
                    throw new BackendException("entity already exists", ErrorCode.TransactionConcurrentInputsProblem);
                }
            }

            foreach (var batch in spentOutputs.Batch(100, p => p.ToList()))
            {
                try
                {
                    await _storage.InsertAsync(batch.Select(SpentOutputTableEntity.ByTransactionHash.Create));
                    await _storage.InsertAsync(batch.Select(SpentOutputTableEntity.ByDateTime.Create));
                }
                catch (AggregateException e)
                {
                    var exception = e.InnerExceptions[0] as StorageException;
                    await ThrowIfBackend(exception);
                    throw;
                }
                catch (StorageException e)
                {
                    await ThrowIfBackend(e);
                    throw;
                }

            }
        }

        public async Task<IEnumerable<ISpentOutput>> GetUnspentOutputs(IEnumerable<ISpentOutput> outputs)
        {
            var enumerable = outputs.ToArray();

            var dbOutputs = await _storage.GetDataAsync(SpentOutputTableEntity.ByTransactionHash.GeneratePartitionKey(), enumerable.Select(x => SpentOutputTableEntity.ByTransactionHash.GenerateRowKey(x.TransactionHash, x.N)), 50);

            var setOfSpentRowKeys = new HashSet<string>(dbOutputs.Select(x => x.RowKey));

            return enumerable.Where(x => !setOfSpentRowKeys.Contains(SpentOutputTableEntity.ByTransactionHash.GenerateRowKey(x.TransactionHash, x.N)));
        }

        public async Task DeleteOldOutputs(DateTime boun)
        {
            do
            {
                var outputsToRemove = (await GetOldOutputs(boun, 10)).ToList();
                if (!outputsToRemove.Any())
                {
                    return;
                }

                await DeleteOutputs(outputsToRemove);
            } while (true);
        }

        private async Task DeleteOutputs(IEnumerable<ISpentOutput> outputs)
        {
            foreach (var output in outputs)
            {
                await _storage.DeleteAsync(SpentOutputTableEntity.ByTransactionHash.Create(output));
                await _storage.DeleteAsync(SpentOutputTableEntity.ByDateTime.Create(output));
            }
        }

        private async Task<IEnumerable<ISpentOutput>> GetOldOutputs(DateTime bound, int count)
        {
            return (await _storage.GetTopRecordsAsync(SpentOutputTableEntity.ByDateTime.GeneratePartition(), count))
                .Where(p=>p.InsertedAt <= bound)
                .ToList();
        }
    }
}
