using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashOut;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashIn
{
    public class CashInOperationEntity : TableEntity, ICashInOperation
    {
        public string OperationId { get; set; }
        public DateTime DetectedAt { get; set; }
        public string DestinationAddress { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }
        public string SourceAddress { get; set; }
        public string TxHash { get; set; }
        public bool MoneyTransferredToHotWallet { get; set; }
        public DateTime? MoneyTransferredToHotWalletAt { get; set; }


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

            public static CashInOperationEntity Create(ICashInOperation source)
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

            public static CashInOperationEntity Create(ICashInOperation source)
            {
                return Map(source, GeneratePartitionKey(),
                    GenerateRowKey(source.DetectedAt));
            }
        }

        public static class ByOperationId
        {
            public static string GeneratePartitionKey()
            {
                return "ByOperationId";
            }

            public static string GenerateRowKey(string operationId)
            {
                return operationId;
            }

            public static CashInOperationEntity Create(ICashInOperation source)
            {
                return Map(source, GeneratePartitionKey(),
                    GenerateRowKey(source.OperationId));
            }
        }


        private static CashInOperationEntity Map(ICashInOperation source, string partitionKey, string rowKey)
        {
            return new CashInOperationEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                SourceAddress = source.SourceAddress,
                OperationId = source.OperationId,
                AssetId = source.OperationId,
                Amount = source.Amount,
                DetectedAt = source.DetectedAt,
                DestinationAddress = source.DestinationAddress,
                TxHash = source.TxHash,
                MoneyTransferredToHotWallet = source.MoneyTransferredToHotWallet,
                MoneyTransferredToHotWalletAt = source.MoneyTransferredToHotWalletAt
            };
        }
    }

    public class CashInOperationRepository: ICashInOperationRepository
    {
        private readonly INoSQLTableStorage<CashInOperationEntity> _storage;

        public CashInOperationRepository(INoSQLTableStorage<CashInOperationEntity> storage)
        {
            _storage = storage;
        }

        public async Task Insert(ICashInOperation operation)
        {
            await _storage.InsertAsync(CashInOperationEntity.ByTxHash.Create(operation));
            await _storage.InsertAsync(CashInOperationEntity.ByDateTime.Create(operation));
            await _storage.InsertAsync(CashInOperationEntity.ByOperationId.Create(operation));
        }

        public async Task<ICashInOperation> GetByOperationId(string operationId)
        {
            return await _storage.GetDataAsync(CashInOperationEntity.ByOperationId.GeneratePartitionKey(),
                CashInOperationEntity.ByOperationId.GenerateRowKey(operationId));
        }

        private async Task<IEnumerable<ICashInOperation>> GetOldOperations(DateTime bound, int count)
        {
            return (await _storage.GetTopRecordsAsync(CashInOperationEntity.ByDateTime.GeneratePartitionKey(), count))
                .Where(p => p.DetectedAt <= bound)
                .ToList();
        }

        public async Task DeleteOperations(IEnumerable<ICashInOperation> operations)
        {
            foreach (var op in operations)
            {
                await _storage.DeleteAsync(CashInOperationEntity.ByDateTime.Create(op));
                await _storage.DeleteAsync(CashInOperationEntity.ByOperationId.Create(op));
                await _storage.DeleteAsync(CashInOperationEntity.ByTxHash.Create(op));
            }
        }

        public async Task SetMoneyTransferred(string operationId, DateTime completedAt)
        {
            var op = await GetByOperationId(operationId);

            if (op == null)
            {
                throw new BackendException($"Operation {operationId} not found", ErrorCode.CashOutOperationNotFound);
            }

            CashInOperationEntity Update(CashInOperationEntity entity)
            {
                entity.MoneyTransferredToHotWallet = true;
                entity.MoneyTransferredToHotWalletAt = completedAt;

                return entity;
            }

            await _storage.ReplaceAsync(CashInOperationEntity.ByOperationId.GeneratePartitionKey(),
                CashInOperationEntity.ByOperationId.GenerateRowKey(op.OperationId),
                Update);

            await _storage.ReplaceAsync(CashInOperationEntity.ByTxHash.GeneratePartitionKey(),
                CashInOperationEntity.ByTxHash.GenerateRowKey(op.TxHash),
                Update);

            await _storage.ReplaceAsync(CashInOperationEntity.ByDateTime.GeneratePartitionKey(),
                CashInOperationEntity.ByDateTime.GenerateRowKey(op.DetectedAt),
                Update);
        }


        public async Task DeleteOldOperations(DateTime bound)
        {
            do
            {
                var outputsToRemove = (await GetOldOperations(bound, 10)).ToList();
                if (!outputsToRemove.Any())
                {
                    return;
                }

                await DeleteOperations(outputsToRemove);
            } while (true);
        }
    }
}
