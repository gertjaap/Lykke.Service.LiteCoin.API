using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashOut
{
    public class PendingCashOutNotificationTableEntity:TableEntity, IPendingCashOutNotification
    {
        public static string GeneratePartitionKey()
        {
            return "CON";
        }

        public static string GenerateRowKey(string operationId)
        {
            return operationId;
        }
        
        public string OperationId { get; set;}

        public DateTime StartedAt { get; set;}

        public string ClientWalletId { get; set;}

        public string AssetId { get; set;}

        public decimal Amount { get; set;}

        public string DestinationAddress { get; set;}

        public string TxHash { get; set;}

        public string Status { get; set; }

        CashOutStatusType IPendingCashOutNotification.Status => Enum.Parse<CashOutStatusType>(Status);


        public static PendingCashOutNotificationTableEntity Create(IPendingCashOutNotification source)
        {
            return new PendingCashOutNotificationTableEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(source.OperationId),
                OperationId = source.OperationId,
                AssetId = source.AssetId,
                Amount = source.Amount,
                TxHash = source.TxHash,
                DestinationAddress = source.DestinationAddress,
                StartedAt = source.StartedAt,
                ClientWalletId = source.ClientWalletId,
                Status = source.Status.ToString()
            };
        }
    }

    public class PendingCashOutNotificationRepository: IPendingCashOutNotificationRepository
    {
        private readonly INoSQLTableStorage<PendingCashOutNotificationTableEntity> _storage;

        public PendingCashOutNotificationRepository(INoSQLTableStorage<PendingCashOutNotificationTableEntity> storage)
        {
            _storage = storage;
        }

        public Task InsertOrReplace(IPendingCashOutNotification notification)
        {
            return _storage.InsertOrReplaceAsync(PendingCashOutNotificationTableEntity.Create(notification));
        }

        public async Task<IEnumerable<IPendingCashOutNotification>> GetAll()
        {
            return await _storage.GetDataAsync(PendingCashOutNotificationTableEntity.GeneratePartitionKey());
        }

        public async Task RemoveBatch(IEnumerable<string> operationIds)
        {
            foreach (var operationId in operationIds)
            {
                await _storage.DeleteIfExistAsync(PendingCashOutNotificationTableEntity.GeneratePartitionKey(),
                    PendingCashOutNotificationTableEntity.GenerateRowKey(operationId));
            }
        }
    }
}
