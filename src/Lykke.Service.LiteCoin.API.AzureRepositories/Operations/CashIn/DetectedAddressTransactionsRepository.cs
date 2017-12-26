using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashIn
{
    public class DetectedAddressTransactionEntity : TableEntity, IDetectedAddressTransaction
    {
        public string TransactionHash { get; set;}
        public string Address { get; set;}
        public DateTime InsertedAt { get; set;}

        public static string GeneratePartitionKey(string address)
        {
            return address;
        }

        public static string GenerateRowKey(string txHash)
        {
            return txHash;
        }

        public static DetectedAddressTransactionEntity Create(IDetectedAddressTransaction source)
        {
            return new DetectedAddressTransactionEntity
            {
                Address = source.Address,
                TransactionHash = source.TransactionHash,
                InsertedAt = source.InsertedAt,
                RowKey = GenerateRowKey(source.TransactionHash),
                PartitionKey = GeneratePartitionKey(source.Address)
            };
        }
    }

    public class DetectedAddressTransactionsRepository: IDetectedAddressTransactionsRepository
    {
        private readonly INoSQLTableStorage<DetectedAddressTransactionEntity> _storage;

        public DetectedAddressTransactionsRepository(INoSQLTableStorage<DetectedAddressTransactionEntity> storage)
        {
            _storage = storage;
        }

        public Task Insert(IEnumerable<IDetectedAddressTransaction> transactions)
        {
            return _storage.InsertAsync(transactions.Select(DetectedAddressTransactionEntity.Create));
        }

        public async Task<IEnumerable<IDetectedAddressTransaction>> GetTxsForAddress(string address)
        {
            return await _storage.GetDataAsync(DetectedAddressTransactionEntity.GeneratePartitionKey(address));
        }
    }
}
