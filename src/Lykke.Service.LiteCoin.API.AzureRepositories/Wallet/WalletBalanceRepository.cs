using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.AzureRepositories.Helpers;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Wallet
{
    public class WalletBalanceEntity : TableEntity, IWalletBalance
    {
        public string Address { get; set; }
        public long BalanceSatoshi { get; set; }
        public DateTime Updated { get; set; }

        public static string GeneratePartitionKey()
        {
            return "ByAddress";
        }

        public static string GenerateRowKey(string address)
        {
            return address;
        }

        public static WalletBalanceEntity Create(IWalletBalance source)
        {
            return new WalletBalanceEntity
            {
                Address = source.Address,
                BalanceSatoshi = source.BalanceSatoshi,
                RowKey = GenerateRowKey(source.Address),
                PartitionKey = GeneratePartitionKey(),
                Updated = source.Updated
            };
        }
    }

    public class WalletBalanceRepository: IWalletBalanceRepository
    {
        private readonly INoSQLTableStorage<WalletBalanceEntity> _storage;

        public WalletBalanceRepository(INoSQLTableStorage<WalletBalanceEntity> storage)
        {
            _storage = storage;
        }

        public Task InsertOrReplace(IWalletBalance balance)
        {
            return _storage.InsertOrReplaceAsync(WalletBalanceEntity.Create(balance));
        }

        public Task DeleteIfExist(string address)
        {
            return _storage.DeleteIfExistAsync(WalletBalanceEntity.GeneratePartitionKey(),
                WalletBalanceEntity.GenerateRowKey(address));
        }

        public async Task<IEnumerable<IWalletBalance>> Get(int skip, int take)
        {
            return await _storage.GetPagedResult(WalletBalanceEntity.GeneratePartitionKey(), skip, take);
        }
    }
}
