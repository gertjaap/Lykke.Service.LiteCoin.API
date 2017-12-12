using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.CashOut
{
    public class TrackedCashoutTransactionEntity : TableEntity, ICashoutTransaction
    {
        public string TxHash { get; set; }
        public string OperationId { get; set; }
        public DateTime InsertedAt { get; set; }

        public static string CreateRowKey(string txHash)
        {
            return txHash;
        }

        public static string CreatePartitionKey()
        {
            return "TCTH";
        }

        public static TrackedCashoutTransactionEntity Create(ICashoutTransaction source)
        {
            return new TrackedCashoutTransactionEntity
            {
                OperationId = source.OperationId,
                InsertedAt = source.InsertedAt,
                PartitionKey = CreatePartitionKey(),
                RowKey = CreateRowKey(source.TxHash),
                TxHash = source.TxHash
            };
        }
    }
    public class PendingCashoutTransactionRepository: IPendingCashoutTransactionRepository
    {
        private readonly INoSQLTableStorage<TrackedCashoutTransactionEntity> _storage;

        public PendingCashoutTransactionRepository(INoSQLTableStorage<TrackedCashoutTransactionEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<ICashoutTransaction>> GetAll()
        {
            return await _storage.GetDataAsync(TrackedCashoutTransactionEntity.CreatePartitionKey());
        }

        public Task Insert(ICashoutTransaction tx)
        {
            return _storage.InsertOrReplaceAsync(TrackedCashoutTransactionEntity.Create(tx));
        }

        public Task Remove(string txHash)
        {
            return _storage.DeleteAsync(TrackedCashoutTransactionEntity.CreatePartitionKey(), TrackedCashoutTransactionEntity.CreateRowKey(txHash));
        }
    }
}
