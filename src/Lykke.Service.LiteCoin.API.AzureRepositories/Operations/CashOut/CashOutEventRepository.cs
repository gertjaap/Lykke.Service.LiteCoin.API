using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashOut
{
    public class CashOutEventTableEntity : TableEntity, ICashOutEvent
    {
        CashOutEventType ICashOutEvent.Type => Enum.Parse<CashOutEventType>(Type);

        public string Type { get; set; }

        public DateTime DateTime { get; set; }
        public string OperationId { get; set; }

        public static string GeneratePartitionKey(string operationId)
        {
            return operationId;
        }

        public static string GenerateRowKey(CashOutEventType type)
        {
            return type.ToString();
        }

        public static CashOutEventTableEntity Create(ICashOutEvent source)
        {
            return new CashOutEventTableEntity
            {
                DateTime = source.DateTime,
                OperationId = source.OperationId,
                PartitionKey = GeneratePartitionKey(source.OperationId),
                RowKey = GenerateRowKey(source.Type)
            };
        }
    }

    public class CashOutEventRepository: ICashOutEventRepository
    {
        private readonly INoSQLTableStorage<CashOutEventTableEntity> _storage;

        public CashOutEventRepository(INoSQLTableStorage<CashOutEventTableEntity> storage)
        {
            _storage = storage;
        }

        public Task InsertEvent(ICashOutEvent cashOutEvent)
        {
            return _storage.InsertAsync(CashOutEventTableEntity.Create(cashOutEvent));
        }

        public async Task<bool> Exist(string operationId, CashOutEventType type)
        {
            return await _storage.GetDataAsync(CashOutEventTableEntity.GeneratePartitionKey(operationId),
                       CashOutEventTableEntity.GenerateRowKey(type)) != null;
        }
    }
}
