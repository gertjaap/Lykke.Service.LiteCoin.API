using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashOut
{
    public class PendingCashOutEventTableEntity:TableEntity, IPendingCashOutEvent
    {
        public Guid OperationId { get; set; }

        public DateTime StartedAt { get; set; }

        public string ClientAddress { get; set; }

        public string AssetId { get; set; }

        public long Amount { get; set; }

        public string DestinationAddress { get; set; }

        public string TxHash { get; set; }

        public string Status { get; set; }
        PendingCashOutEventStatusType IPendingCashOutEvent.Status => GetStatusType();
        public PendingCashOutEventStatusType GetStatusType()
        {
            return Enum.Parse<PendingCashOutEventStatusType>(Status);
        }
        
        public static class ByStatus
        {
            public static string GeneratePartitionKey(PendingCashOutEventStatusType status)
            {
                return status.ToString();
            }

            public static string GenerateRowKey(Guid operationId)
            {
                return operationId.ToString();
            }

            public static PendingCashOutEventTableEntity Create(IPendingCashOutEvent source)
            {
                return Map(source, GeneratePartitionKey(source.Status), GenerateRowKey(source.OperationId));
            }
        }



        public static PendingCashOutEventTableEntity Map(IPendingCashOutEvent source, string partitionKey, string rowKey)
        {
            return new PendingCashOutEventTableEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                OperationId = source.OperationId,
                AssetId = source.AssetId,
                Amount = source.Amount,
                TxHash = source.TxHash,
                DestinationAddress = source.DestinationAddress,
                StartedAt = source.StartedAt,
                ClientAddress = source.ClientAddress,
                Status = source.Status.ToString()
            };
        }


    }
    public class PendingCashOutEventRepository: IPendingCashOutEventRepository
    {
        private readonly INoSQLTableStorage<PendingCashOutEventTableEntity> _storage;

        public PendingCashOutEventRepository(INoSQLTableStorage<PendingCashOutEventTableEntity> storage)
        {
            _storage = storage;
        }
        
        public async Task Insert(IPendingCashOutEvent @event)
        {
            await _storage.InsertAsync(PendingCashOutEventTableEntity.ByStatus.Create(@event));
        }

        public async Task<IEnumerable<IPendingCashOutEvent>> GetAll(PendingCashOutEventStatusType status, int count)
        {
            return await _storage.GetTopRecordsAsync(PendingCashOutEventTableEntity.ByStatus.GeneratePartitionKey(status), count);
        }

        public async Task DeleteBatchIfExist(PendingCashOutEventStatusType status, IEnumerable<Guid> operationIds)
        {
            foreach (var operationId in operationIds)
            {
                await _storage.DeleteIfExistAsync(PendingCashOutEventTableEntity.ByStatus.GeneratePartitionKey(status),
                    PendingCashOutEventTableEntity.ByStatus.GenerateRowKey(operationId));
            }
        }
    }
}
