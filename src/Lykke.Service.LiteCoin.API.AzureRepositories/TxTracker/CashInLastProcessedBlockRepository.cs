using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.BlockChainTracker;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.TxTracker
{
    public class LastCashInProcessedBlockEntity : TableEntity
    {
        public int Height { get; set; }
        public static string GenerateRowKey()
        {
            return "LPB";
        }

        public static string GeneratePartitionKey()
        {
            return "LPB";
        }

        public static LastCashInProcessedBlockEntity Create(int height)
        {
            return new LastCashInProcessedBlockEntity
            {
                RowKey = GenerateRowKey(),
                PartitionKey = GeneratePartitionKey(),
                Height = height
            };
        }
    }

    public class CashInLastProcessedBlockRepository: ICashInLastProcessedBlockRepository
    {
        private readonly INoSQLTableStorage<LastCashInProcessedBlockEntity> _storage;

        public CashInLastProcessedBlockRepository(INoSQLTableStorage<LastCashInProcessedBlockEntity> storage)
        {
            _storage = storage;
        }

        public async Task<int> GetLastProcessedBlockHeight()
        {
            return (await _storage.GetDataAsync(
                LastCashInProcessedBlockEntity.GeneratePartitionKey(),
                LastCashInProcessedBlockEntity.GenerateRowKey()))?.Height ?? 0;
        }

        public Task SetLastProcessedBlockHeight(int height)
        {
            return _storage.InsertOrReplaceAsync(LastCashInProcessedBlockEntity.Create(height));
        }
    }
}
