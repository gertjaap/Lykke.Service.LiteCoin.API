using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashOut
{
    public class PendingCashoutTransactionEntity : TableEntity, ICashoutTransaction
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

        public static PendingCashoutTransactionEntity Create(ICashoutTransaction source)
        {
            return new PendingCashoutTransactionEntity
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
        private readonly INoSQLTableStorage<PendingCashoutTransactionEntity> _storage;

        public PendingCashoutTransactionRepository(INoSQLTableStorage<PendingCashoutTransactionEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<ICashoutTransaction>> GetAll()
        {
            var t = await _storage.GetDataAsync("TCTH", "4eb687f5dac0091b0ac7883f17be385acb424171f71a01a03eed65b76c6aa22a");
            return await _storage.GetDataAsync(PendingCashoutTransactionEntity.CreatePartitionKey());
        }

        public Task Insert(ICashoutTransaction tx)
        {
            return _storage.InsertOrReplaceAsync(PendingCashoutTransactionEntity.Create(tx));
        }

        public Task Remove(string txHash)
        {
            return _storage.DeleteAsync(PendingCashoutTransactionEntity.CreatePartitionKey(), PendingCashoutTransactionEntity.CreateRowKey(txHash));
        }
    }
}
