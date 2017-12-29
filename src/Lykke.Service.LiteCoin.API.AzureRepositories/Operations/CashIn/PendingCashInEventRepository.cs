using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashIn
{
    public class PendingCashInEventTableEntity:TableEntity, IPendingCashInEvent
    {
        public Guid OperationId { get; set; }
        public DateTime DetectedAt { get; set; }
        public string DestinationAddress { get; set; }
        public long Amount { get; set; }
        public string AssetId { get; set; }
        public string SourceAddress { get; set; }
        public string TxHash { get; set; }
        public string Status { get; set; }
        PendingCashInEventStatusType IPendingCashInEvent.Status => GetStatusType();
        public PendingCashInEventStatusType GetStatusType()
        {
            return Enum.Parse<PendingCashInEventStatusType>(Status);
        }


        public static class ByStatus
        {
            public static string GeneratePartitionKey(PendingCashInEventStatusType status)
            {
                return status.ToString();
            }

            public static string GenerateRowKey(Guid operationId)
            {
                return operationId.ToString();
            }

            public static PendingCashInEventTableEntity Create(IPendingCashInEvent source)
            {
                return Map(source, GeneratePartitionKey(source.Status), GenerateRowKey(source.OperationId));
            }
        }



        public static PendingCashInEventTableEntity Map(IPendingCashInEvent source, string partitionKey, string rowKey)
        {
            return new PendingCashInEventTableEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
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
    public class PendingCashInEventRepository: IPendingCashInEventRepository
    {
        private readonly INoSQLTableStorage<PendingCashInEventTableEntity> _storage;

        public PendingCashInEventRepository(INoSQLTableStorage<PendingCashInEventTableEntity> storage)
        {
            _storage = storage;
        }
        
        public async Task Insert(IPendingCashInEvent @event){
            
            await _storage.InsertAsync(PendingCashInEventTableEntity.ByStatus.Create(@event));
        }

        public async Task<IEnumerable<IPendingCashInEvent>> GetAll(PendingCashInEventStatusType status, int count)
        {
            return (await _storage.GetDataAsync(PendingCashInEventTableEntity.ByStatus.GeneratePartitionKey(status))).Take(count); // todo implement Take logic in db
        }

        public async Task DeleteBatchIfExist(PendingCashInEventStatusType status, IEnumerable<Guid> operationIds)
        {
            foreach (var operationId in operationIds)
            {
                await _storage.DeleteIfExistAsync(PendingCashInEventTableEntity.ByStatus.GeneratePartitionKey(status),
                    PendingCashInEventTableEntity.ByStatus.GenerateRowKey(operationId));
            }
        }
    }
}
