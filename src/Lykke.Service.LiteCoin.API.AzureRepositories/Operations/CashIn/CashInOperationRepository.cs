using System;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashIn
{
    public class CashInOperationEntity : TableEntity, ICashInOperation
    {
        public string OperationId { get; set; }
        public DateTime DetectedAt { get; set; }
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }
        public string Address { get; set; }
        public string TxHash { get; set; }

        public static string GeneratePartitionKey()
        {
            return "CIO";
        }

        public static string GenerateRowKey(string txHash)
        {
            return txHash;
        }

        public static CashInOperationEntity Create(ICashInOperation source)
        {
            return new CashInOperationEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(source.TxHash),
                Address = source.Address,
                OperationId = source.OperationId,
                AssetId = source.OperationId,
                Amount = source.Amount,
                DetectedAt = source.DetectedAt
            };
        }
    }

    public class CashInOperationRepository: ICashInOperationRepository
    {
        private readonly INoSQLTableStorage<CashInOperationEntity> _storage;

        public CashInOperationRepository(INoSQLTableStorage<CashInOperationEntity> storage)
        {
            _storage = storage;
        }

        public Task Insert(ICashInOperation operation)
        {
            return _storage.InsertAsync(CashInOperationEntity.Create(operation));
        }
    }
}
