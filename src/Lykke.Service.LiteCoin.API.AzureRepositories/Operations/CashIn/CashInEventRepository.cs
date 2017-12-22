using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashIn
{
    public class CashInEventTableEntity : TableEntity, ICashInEvent
    {
        CashInEventType ICashInEvent.Type => Enum.Parse<CashInEventType>(Type);

        public string Type { get; set; }

        public DateTime DateTime { get; set; }
        public string OperationId { get; set; }

        public static string GeneratePartitionKey(string operationId)
        {
            return operationId;
        }

        public static string GenerateRowKey(CashInEventType type)
        {
            return type.ToString();
        }

        public static CashInEventTableEntity Create(ICashInEvent source)
        {
            return new CashInEventTableEntity
            {
                DateTime = source.DateTime,
                OperationId = source.OperationId,
                PartitionKey = GeneratePartitionKey(source.OperationId),
                RowKey = GenerateRowKey(source.Type)
            };
        }
    }

    public class CashInEventRepository: ICashInEventRepository
    {
        private readonly INoSQLTableStorage<CashInEventTableEntity> _storage;

        public CashInEventRepository(INoSQLTableStorage<CashInEventTableEntity> storage)
        {
            _storage = storage;
        }

        public Task InsertEvent(ICashInEvent cashInEvent)
        {
            return _storage.InsertAsync(CashInEventTableEntity.Create(cashInEvent));
        }

        public async Task<bool> Exist(string operationId, CashInEventType type)
        {
            return await _storage.GetDataAsync(CashInEventTableEntity.GeneratePartitionKey(operationId),
                       CashInEventTableEntity.GenerateRowKey(type)) != null;
        }
    }
}
