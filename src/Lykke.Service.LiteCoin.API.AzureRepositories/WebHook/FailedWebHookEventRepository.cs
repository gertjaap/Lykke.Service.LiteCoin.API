using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.LiteCoin.API.Core.WebHook;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.WebHook
{
    internal class FailedWebHookEventEntity : TableEntity, IFailedWebHookEvent
    {
        public static string GeneratePartitionKey()
        {

            return "FWH";
        }

        public static string GenerateRowKey(string operationId)
        {
            return operationId;
        }

        public static FailedWebHookEventEntity Create(IFailedWebHookEvent source)
        {
            return new FailedWebHookEventEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(source.OperationId),
                OperationId = source.OperationId,
                Context = source.Context,
                WebHookType = source.WebHookType.ToString()
            };
        }

        public string OperationId { get; set; }
        public string Context { get; set; }

        public string WebHookType { get; set; }
        WebHookType IFailedWebHookEvent.WebHookType => Enum.Parse<WebHookType>(WebHookType);
    }

    internal class FailedWebHookEventRepository: IFailedWebHookEventRepository
    {
        private readonly INoSQLTableStorage<FailedWebHookEventEntity> _storage;

        public FailedWebHookEventRepository(INoSQLTableStorage<FailedWebHookEventEntity> storage)
        {
            _storage = storage;
        }

        public Task Insert(IFailedWebHookEvent ev)
        {
            return _storage.InsertOrReplaceAsync(FailedWebHookEventEntity.Create(ev));
        }

        public Task DeleteIfExist(string operationId)
        {
            return _storage.DeleteIfExistAsync(FailedWebHookEventEntity.GeneratePartitionKey(),
                FailedWebHookEventEntity.GenerateRowKey(operationId));
        }

        public async Task<IEnumerable<IFailedWebHookEvent>> GetAll()
        {
            return await _storage.GetDataAsync(FailedWebHookEventEntity.GeneratePartitionKey());
        }
    }
}
