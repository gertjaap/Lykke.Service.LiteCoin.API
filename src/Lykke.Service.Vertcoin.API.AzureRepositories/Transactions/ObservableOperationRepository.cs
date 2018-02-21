﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.Vertcoin.API.AzureRepositories.Helpers;
using Lykke.Service.Vertcoin.API.Core.ObservableOperation;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.Vertcoin.API.AzureRepositories.Transactions
{
    public class ObservableOperationEntity : TableEntity, IObservableOperation
    {
        BroadcastStatus IObservableOperation.Status => Enum.Parse<BroadcastStatus>(Status);

        public string Status { get; set; }

        public Guid OperationId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string AssetId { get; set; }
        public long AmountSatoshi { get; set; }
        public long FeeSatoshi { get; set; }
        public DateTime Updated { get; set; }
        public string TxHash { get; set; }

        public static ObservableOperationEntity Map(string partitionKey, string rowKey,
            IObservableOperation source)
        {
            return new ObservableOperationEntity
            {
                OperationId = source.OperationId,
                PartitionKey = partitionKey,
                RowKey = rowKey,
                FromAddress = source.FromAddress,
                AssetId = source.AssetId,
                ToAddress = source.ToAddress,
                AmountSatoshi = source.AmountSatoshi,
                Status = source.Status.ToString(),
                Updated = source.Updated,
                TxHash = source.TxHash,
                FeeSatoshi = source.FeeSatoshi
            };
        }

        public static class ByOperationId 
        {
            public static string GeneratePartitionKey()
            {
                return "ByOperationId";
            }

            public static string GenerateRowKey(Guid operationId)
            {
                return operationId.ToString();
            }

            public static ObservableOperationEntity Create(IObservableOperation source)
            {
                return Map(GeneratePartitionKey(), GenerateRowKey(source.OperationId), source);
            }
        }
    }

    public class ObservableOperationRepository: IObservableOperationRepository
    {
        private readonly INoSQLTableStorage<ObservableOperationEntity> _storage;

        public ObservableOperationRepository(INoSQLTableStorage<ObservableOperationEntity> storage)
        {
            _storage = storage;
        }
        

        public async Task InsertOrReplace(IObservableOperation tx)
        {
            await _storage.InsertOrReplaceAsync(ObservableOperationEntity.ByOperationId.Create(tx));
        }

        public async Task DeleteIfExist(params Guid[] operationIds)
        {
            foreach (var operationId in operationIds)
            {
                await _storage.DeleteIfExistAsync(ObservableOperationEntity.ByOperationId.GeneratePartitionKey(),
                    ObservableOperationEntity.ByOperationId.GenerateRowKey(operationId));
            }
        }

        public async Task<IObservableOperation> GetById(Guid opId)
        {
            return await _storage.GetDataAsync(ObservableOperationEntity.ByOperationId.GeneratePartitionKey(),
                UnconfirmedTransactionEntity.GenerateRowKey(opId));
        }
    }
}
