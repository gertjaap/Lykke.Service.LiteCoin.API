using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashIn
{
    public class PendingCashInNotificationTableEntity:TableEntity, IPendingCashInNotification
    {
        public string OperationId { get; set; }
        public DateTime DetectedAt { get; set; }
        public string DestinationAddress { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }
        public string SourceAddress { get; set; }
        public string TxHash { get; set; }

        public static string GeneratePartitionKey()
        {
            return "CIN";
        }

        public static string GenerateRowKey(string operationId)
        {
            return operationId;
        }

        public static PendingCashInNotificationTableEntity Create(IPendingCashInNotification source)
        {
            return new PendingCashInNotificationTableEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(source.OperationId),
                OperationId = source.OperationId,
                AssetId = source.AssetId,
                Amount = source.Amount,
                TxHash = source.TxHash,
                DetectedAt = source.DetectedAt,
                DestinationAddress = source.DestinationAddress,
                SourceAddress = source.SourceAddress
            };
        }
    }
    public class PendingCashInNotificationRepository: IPendingCashInNotificationRepository
    {
        private readonly INoSQLTableStorage<PendingCashInNotificationTableEntity> _storage;

        public PendingCashInNotificationRepository(INoSQLTableStorage<PendingCashInNotificationTableEntity> storage)
        {
            _storage = storage;
        }

        public Task Insert(IPendingCashInNotification notification)
        {
            return _storage.InsertOrReplaceAsync(PendingCashInNotificationTableEntity.Create(notification));
        }

        public async Task<IEnumerable<IPendingCashInNotification>> GetAll()
        {
            return await _storage.GetDataAsync(PendingCashInNotificationTableEntity.GeneratePartitionKey());
        }

        public async Task RemoveBatch(IEnumerable<string> operationIds)
        {
            foreach (var operationId in operationIds)
            {
                await _storage.DeleteIfExistAsync(PendingCashInNotificationTableEntity.GeneratePartitionKey(),
                    PendingCashInNotificationTableEntity.GenerateRowKey(operationId));
            }
        }
    }
}
