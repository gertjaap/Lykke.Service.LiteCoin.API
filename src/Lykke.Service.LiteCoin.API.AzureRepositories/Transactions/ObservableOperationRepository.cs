using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.AzureRepositories.Helpers;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Core.PendingEvent;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Transactions
{
    public class ObservableOperationEntity : TableEntity, IObservableTransactionData
    {
        BroadcastStatus IObservableTransactionData.Status => Enum.Parse<BroadcastStatus>(Status);

        public string Status { get; set; }

        public Guid OperationId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string AssetId { get; set; }
        public long AmountSatoshi { get; set; }
        public DateTime Updated { get; set; }

        public static ObservableOperationEntity Map(string partitionKey, string rowKey,
            IObservableTransactionData source)
        {
            return new ObservableOperationEntity
            {
                OperationId = source.OperationId,
                PartitionKey = partitionKey,
                RowKey = rowKey,
                FromAddress = source.FromAddress,
                AssetId = source.AssetId,
                ToAddress = source.ToAddress,
                AmountSatoshi = source.AmountSatoshi,
                Status = source.Status.ToString(),
                Updated = source.Updated
            };
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

            public static ObservableOperationEntity Create(IObservableTransactionData source)
            {
                return Map(GeneratePartitionKey(), GenerateRowKey(source.OperationId), source);
            }
        }

        public static class ByStatus
        {
            public static string GeneratePartitionKey(BroadcastStatus status)
            {
                return status.ToString();
            }

            public static string GenerateRowKey(Guid operationId)
            {
                return operationId.ToString();
            }

            public static ObservableOperationEntity Create(IObservableTransactionData source)
            {
                return Map(GeneratePartitionKey(source.Status), GenerateRowKey(source.OperationId), source);
            }
        }
    }

    public class ObservableOperationRepository: IObservableOperationRepository
    {
        private readonly INoSQLTableStorage<ObservableOperationEntity> _storage;

        public ObservableOperationRepository(INoSQLTableStorage<ObservableOperationEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<IObservableTransactionData>> Get(BroadcastStatus status, int skip, int take)
        {
            return await _storage.GetPagedResult(ObservableOperationEntity.ByStatus.GeneratePartitionKey(status), skip, take);
        }

        public async Task Insert(IObservableTransactionData tx)
        {
            await _storage.InsertAsync(ObservableOperationEntity.ByOperationId.Create(tx));
            await _storage.InsertAsync(ObservableOperationEntity.ByStatus.Create(tx));
        }

        public async Task ChangeStatus(Guid operationId, BroadcastStatus status)
        {
            var dbEntity = await GetById(operationId);

            if (dbEntity != null)
            {
                await DeleteIfExist(dbEntity);

                dbEntity.Status = status.ToString();
                dbEntity.Updated = DateTime.UtcNow;

                await Insert(dbEntity);
            }
        }

        public async Task DeleteIfExist(params Guid[] operationIds)
        {
            foreach (var operationId in operationIds)
            {
                var entity = await GetById(operationId);

                await DeleteIfExist(entity);
            }
        }

        public async Task DeleteIfExist(IObservableTransactionData source)
        {
            if (source != null)
            {
                await _storage.DeleteIfExistAsync(ObservableOperationEntity.ByOperationId.GeneratePartitionKey(),
                    ObservableOperationEntity.ByOperationId.GenerateRowKey(source.OperationId));

                await _storage.DeleteIfExistAsync(ObservableOperationEntity.ByStatus.GeneratePartitionKey(source.Status),
                    ObservableOperationEntity.ByStatus.GenerateRowKey(source.OperationId));
            }
        }

        private async Task<ObservableOperationEntity> GetById(Guid operationId)
        {
            return await _storage.GetDataAsync(ObservableOperationEntity.ByOperationId.GeneratePartitionKey(),
                UnconfirmedTransactionEntity.GenerateRowKey(operationId));
        }
    }
}
