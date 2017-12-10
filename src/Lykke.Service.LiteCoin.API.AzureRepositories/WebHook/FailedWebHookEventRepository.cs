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
    internal class FailedWebHookEventEntity : TableEntity
    {
        public string Data { get; set; }
        public static string GeneratePartitionKey()
        {

            return "FWH";
        }

        public static string GenerateRowKey(string operationId)
        {
            return operationId;
        }

        public static FailedWebHookEventEntity Create(object eventData, string operationId)
        {
            return new FailedWebHookEventEntity
            {
                PartitionKey = GeneratePartitionKey(),
                Data = eventData.ToJson(),
                RowKey = GenerateRowKey(operationId)
            };
        }
    }

    internal class FailedWebHookEventRepository: IFailedWebHookEventRepository
    {
        private readonly INoSQLTableStorage<FailedWebHookEventEntity> _storage;

        public FailedWebHookEventRepository(INoSQLTableStorage<FailedWebHookEventEntity> storage)
        {
            _storage = storage;
        }

        public Task Insert(object eventData, string operationId)
        {
            return _storage.InsertOrReplaceAsync(FailedWebHookEventEntity.Create(eventData, operationId));
        }

        public Task DeleteIfExist(string operationId)
        {
            return _storage.DeleteIfExistAsync(FailedWebHookEventEntity.GeneratePartitionKey(),
                FailedWebHookEventEntity.GenerateRowKey(operationId));
        }
    }
}
