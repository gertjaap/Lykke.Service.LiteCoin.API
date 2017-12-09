using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.Operations;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations
{
    public class CashOutOperationTableEntity : TableEntity, ICashOutOperation
    {
        public string OperationId { get; set;}
        public DateTime StartedAt { get; set;}
        public DateTime? CompletedAt { get; set;}
        public string WalletId { get; set;}
        public string AssetId { get; set;}
        public decimal Amount { get; set;}
        public string Address { get; set;}
        public bool Completed { get; set; }

        public static string CreatePartitionKey()
        {
            return "COO";
        }

        public static string CreateRowKey(string operationId)
        {
            return operationId;
        }

        public static CashOutOperationTableEntity Create(ICashOutOperation source)
        {
            return new CashOutOperationTableEntity
            {
                PartitionKey = CreatePartitionKey(),
                RowKey = CreateRowKey(source.OperationId),
                Address = source.Address,
                Amount = source.Amount,
                AssetId = source.AssetId,
                CompletedAt = source.CompletedAt,
                OperationId = source.OperationId,
                StartedAt = source.StartedAt,
                WalletId = source.WalletId,
                Completed = source.Completed
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

        public Task Insert(ICashOutOperation operation)
        {
            return _storage.InsertAsync(CashOutOperationTableEntity.Create(operation));
        }

        public Task SetCompleted(string operationId, DateTime completedAt)
        {
            return _storage.ReplaceAsync(CashOutOperationTableEntity.CreatePartitionKey(),
                CashOutOperationTableEntity.CreateRowKey(operationId),
                entity =>
                {
                    entity.CompletedAt = completedAt;
                    entity.Completed = true;

                    return entity;
                });
        }
    }
}
