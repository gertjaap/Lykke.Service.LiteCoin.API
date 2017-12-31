using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashOut
{
    public class UnconfirmedCashoutTransactionEntity : TableEntity, IUnconfirmedCashoutTransaction
    {
        public string TxHash { get; set; }
        public Guid OperationId { get; set; }
        public DateTime InsertedAt { get; set; }

        public static string CreateRowKey(string txHash)
        {
            return txHash;
        }

        public static string CreatePartitionKey()
        {
            return "TCTH";
        }

        public static UnconfirmedCashoutTransactionEntity Create(IUnconfirmedCashoutTransaction source)
        {
            return new UnconfirmedCashoutTransactionEntity
            {
                OperationId = source.OperationId,
                InsertedAt = source.InsertedAt,
                PartitionKey = CreatePartitionKey(),
                RowKey = CreateRowKey(source.TxHash),
                TxHash = source.TxHash
            };
        }
    }
    public class UnconfirmedCashoutTransactionRepository: IUnconfirmedCashoutTransactionRepository
    {
        private readonly INoSQLTableStorage<UnconfirmedCashoutTransactionEntity> _storage;

        public UnconfirmedCashoutTransactionRepository(INoSQLTableStorage<UnconfirmedCashoutTransactionEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<IUnconfirmedCashoutTransaction>> GetAll()
        {
            return await _storage.GetDataAsync(UnconfirmedCashoutTransactionEntity.CreatePartitionKey());
        }

        public Task InsertOrReplace(IUnconfirmedCashoutTransaction tx)
        {
            return _storage.InsertOrReplaceAsync(UnconfirmedCashoutTransactionEntity.Create(tx));
        }

        public Task Remove(string txHash)
        {
            return _storage.DeleteAsync(UnconfirmedCashoutTransactionEntity.CreatePartitionKey(), UnconfirmedCashoutTransactionEntity.CreateRowKey(txHash));
        }
    }
}
