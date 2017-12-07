using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.TxTracker;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.TxTracker
{
    public class LastProcessedBlockEntity : TableEntity
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

        public static LastProcessedBlockEntity Create(int height)
        {
            return new LastProcessedBlockEntity
            {
                RowKey = GenerateRowKey(),
                PartitionKey = GeneratePartitionKey(),
                Height = height
            };
        }
    }

    public class LastProcessedBlockRepository: ILastProcessedBlockRepository
    {
        private readonly INoSQLTableStorage<LastProcessedBlockEntity> _storage;

        public LastProcessedBlockRepository(INoSQLTableStorage<LastProcessedBlockEntity> storage)
        {
            _storage = storage;
        }

        public async Task<int> GetLastProcessedBlockHeight()
        {
            return (await _storage.GetDataAsync(
                LastProcessedBlockEntity.GeneratePartitionKey(),
                LastProcessedBlockEntity.GenerateRowKey()))?.Height ?? 0;
        }

        public Task SetLastProcessedBlockHeight(int height)
        {
            return _storage.InsertOrReplaceAsync(LastProcessedBlockEntity.Create(height));
        }
    }
}
