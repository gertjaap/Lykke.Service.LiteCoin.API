using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Microsoft.WindowsAzure.Storage.Table;
using MoreLinq;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.TransactionOutput.BroadcastedOutputs
{
    public class BroadcastedOutputTableEntity:TableEntity,IBroadcastedOutput
    {
        public string Address { get; set; }
        public string ScriptPubKey { get; set; }
        public long Amount { get; set; }

        public string TransactionHash { get; set; }

        public int N { get; set; }
        public DateTime InsertedAt { get; set; }

        public static class ByAddress
        {
            public static string GeneratePartition(string address)
            {
                return address;
            }

            public static string GenerateRowKey(string transactionHash, int n)
            {
                return $"{transactionHash}_{n}";
            }

            public static BroadcastedOutputTableEntity Create(IBroadcastedOutput output)
            {
                return Map(output,
                    GeneratePartition(output.Address),
                    GenerateRowKey(output.TransactionHash, output.N));
            }
        }

        public static class ByTransactionHash
        {
            public static string GeneratePartition(string transactionHash)
            {
                return transactionHash;
            }

            public static string GenerateRowKey(int n)
            {
                return n.ToString();
            }

            public static BroadcastedOutputTableEntity Create(IBroadcastedOutput output)
            {
                return Map(output, 
                    GeneratePartition(output.TransactionHash),
                    GenerateRowKey(output.N));
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
                return (DateTime.MaxValue.Ticks - inserted.Ticks).ToString("d19");
            }

            public static BroadcastedOutputTableEntity Create(IBroadcastedOutput output)
            {
                return Map(output, GeneratePartition(),
                    GenerateRowKey(output.InsertedAt));
            }
        }

        private static BroadcastedOutputTableEntity Map(IBroadcastedOutput source, string partitionKey,
            string rowKey)
        {
            return new BroadcastedOutputTableEntity
            {
                RowKey = rowKey,
                PartitionKey = partitionKey,
                TransactionHash = source.TransactionHash,
                N = source.N,
                Address = source.Address,
                Amount = source.Amount,
                ScriptPubKey = source.ScriptPubKey,
                InsertedAt = source.InsertedAt
            };
        }
    }

    public class BroadcastedOutputRepository: IBroadcastedOutputRepository
    {
        private readonly INoSQLTableStorage<BroadcastedOutputTableEntity> _storage;

        public BroadcastedOutputRepository(INoSQLTableStorage<BroadcastedOutputTableEntity> storage)
        {
            _storage = storage;
        }

        public async Task InsertOutputs(IEnumerable<IBroadcastedOutput> outputs)
        {
            foreach (var batch in outputs.Batch(100, p => p.ToList()))
            {
                await _storage.InsertAsync(batch.Select(BroadcastedOutputTableEntity.ByTransactionHash.Create));

                foreach (var addressGroup in batch.GroupBy(o => o.Address))
                {
                    await _storage.InsertAsync(addressGroup.Select(BroadcastedOutputTableEntity.ByAddress.Create));
                }

                await _storage.InsertAsync(batch.Select(BroadcastedOutputTableEntity.ByDateTime.Create));
            }
        }

        public async Task<IEnumerable<IBroadcastedOutput>> GetOutputs(string address)
        {
            return await _storage.GetDataAsync(BroadcastedOutputTableEntity.ByAddress.GeneratePartition(address));
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


        public async Task DeleteOutputs(IEnumerable<IBroadcastedOutput> outputs)
        {
            foreach (var output in outputs)
            {
                await _storage.DeleteAsync(BroadcastedOutputTableEntity.ByDateTime.GeneratePartition(), 
                    BroadcastedOutputTableEntity.ByDateTime.GenerateRowKey(output.InsertedAt));

                await _storage.DeleteAsync(BroadcastedOutputTableEntity.ByTransactionHash.GeneratePartition(output.TransactionHash),
                    BroadcastedOutputTableEntity.ByTransactionHash.GenerateRowKey(output.N));

                await _storage.DeleteAsync(BroadcastedOutputTableEntity.ByAddress.GeneratePartition(output.Address),
                    BroadcastedOutputTableEntity.ByAddress.GenerateRowKey(output.TransactionHash, output.N));
            }
        }

        public async Task DeleteOutput(string transactionHash, int n)
        {
            var output = await _storage.GetDataAsync(BroadcastedOutputTableEntity.ByTransactionHash.GeneratePartition(transactionHash), BroadcastedOutputTableEntity.ByTransactionHash.GenerateRowKey(n));
            if (output != null)
            {
                await DeleteOutputs(new []{output});
            }
        }

        private async Task<IEnumerable<IBroadcastedOutput>> GetOldOutputs(DateTime bound, int count)
        {
            return (await _storage.GetTopRecordsAsync(BroadcastedOutputTableEntity.ByDateTime.GeneratePartition(), count))
                .Where(p => p.InsertedAt <= bound)
                .ToList();
        }
    }
}
