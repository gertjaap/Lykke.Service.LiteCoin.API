using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashOut
{
    public class CashOutOperationTableEntity : TableEntity, ICashOutOperation
    {
        public Guid OperationId { get; set;}
        public DateTime StartedAt { get; set;}
        public string ClientWalletId { get; set;}
        public string AssetId { get; set;}
        public long Amount { get; set;}
        public string DestinationAddress { get; set;}
        public string TxHash { get; set; }

        public static class ByTxHash
        {
            public static string GeneratePartitionKey()
            {
                return "ByTxHash";
            }

            public static string GenerateRowKey(string txHash)
            {
                return txHash;
            }

            public static CashOutOperationTableEntity Create(ICashOutOperation source)
            {
                return Map(source, GeneratePartitionKey(), GenerateRowKey(source.TxHash));
            }

        }

        public static class ByDateTime
        {
            public static string GeneratePartitionKey()
            {
                return "ByDateTime";
            }

            public static string GenerateRowKey(DateTime inserted)
            {
                return (int.MaxValue - inserted.Ticks).ToString("D");
            }

            public static CashOutOperationTableEntity Create(ICashOutOperation source)
            {
                return Map(source, GeneratePartitionKey(),
                    GenerateRowKey(source.StartedAt));
            }
        }

        public static class ByOperationId
        {
            public static string GeneratePartitionKey()
            {
                return "ByOperationId";
            }

            public static string GenerateRowKey(Guid operationId)
            {
                return operationId.ToString();
            }

            public static CashOutOperationTableEntity Create(ICashOutOperation source)
            {
                return Map(source, GeneratePartitionKey(),
                    GenerateRowKey(source.OperationId));
            }
        }

        public static CashOutOperationTableEntity Map(ICashOutOperation source, string partition, string rowKey)
        {
            return new CashOutOperationTableEntity
            {
                PartitionKey = partition,
                RowKey = rowKey,
                DestinationAddress = source.DestinationAddress,
                Amount = source.Amount,
                AssetId = source.AssetId,
                OperationId = source.OperationId,
                StartedAt = source.StartedAt,
                ClientWalletId = source.ClientWalletId,
                TxHash = source.TxHash
            };
        }
    }
    public class CashOutOperationRepository: ICashOutOperationRepository
    {
        private readonly INoSQLTableStorage<CashOutOperationTableEntity> _storage;

        public CashOutOperationRepository(INoSQLTableStorage<CashOutOperationTableEntity> storage)
        {
            _storage = storage;
        }

        public async Task Insert(ICashOutOperation operation)
        {
            await _storage.InsertAsync(CashOutOperationTableEntity.ByOperationId.Create(operation));
            await _storage.InsertAsync(CashOutOperationTableEntity.ByTxHash.Create(operation));
            await _storage.InsertAsync(CashOutOperationTableEntity.ByDateTime.Create(operation));
        }
        

        public async Task<ICashOutOperation> GetByOperationId(Guid operationId)
        {
            return await _storage.GetDataAsync(CashOutOperationTableEntity.ByOperationId.GeneratePartitionKey(),
                CashOutOperationTableEntity.ByOperationId.GenerateRowKey(operationId));
        }

        private async Task<IEnumerable<ICashOutOperation>> GetOldOperations(DateTime bound, int count)
        {
            return (await _storage.GetTopRecordsAsync(CashOutOperationTableEntity.ByDateTime.GeneratePartitionKey(), count))
                .Where(p => p.StartedAt <= bound)
                .ToList();
        }

        public async Task DeleteOperations(IEnumerable<ICashOutOperation> operations)
        {
            foreach (var op in operations)
            {
                await _storage.DeleteAsync(CashOutOperationTableEntity.ByDateTime.Create(op));
                await _storage.DeleteAsync(CashOutOperationTableEntity.ByOperationId.Create(op));
                await _storage.DeleteAsync(CashOutOperationTableEntity.ByTxHash.Create(op));
            }
        }

        public async Task DeleteOldOperations(DateTime boun)
        {
            do
            {
                var outputsToRemove = (await GetOldOperations(boun, 10)).ToList();
                if (!outputsToRemove.Any())
                {
                    return;
                }

                await DeleteOperations(outputsToRemove);
            } while (true);
        }
    }
}
