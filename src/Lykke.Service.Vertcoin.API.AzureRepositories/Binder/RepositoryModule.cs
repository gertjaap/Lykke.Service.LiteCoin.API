﻿using Autofac;
using AzureStorage.Blob;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.Vertcoin.API.AzureRepositories.Asset;
using Lykke.Service.Vertcoin.API.AzureRepositories.Operations;
using Lykke.Service.Vertcoin.API.AzureRepositories.Transactions;
using Lykke.Service.Vertcoin.API.AzureRepositories.Wallet;
using Lykke.Service.Vertcoin.API.Core.Asset;
using Lykke.Service.Vertcoin.API.Core.ObservableOperation;
using Lykke.Service.Vertcoin.API.Core.Operation;
using Lykke.Service.Vertcoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.Vertcoin.API.Core.Transactions;
using Lykke.Service.Vertcoin.API.Core.Wallet;
using Lykke.SettingsReader;

namespace Lykke.Service.Vertcoin.API.AzureRepositories.Binder
{
    public  class RepositoryModule:Module
    {
        private readonly ILog _log;
        private readonly IReloadingManager<VertcoinApiSettings> _settings;
        public RepositoryModule(IReloadingManager<VertcoinApiSettings> settings, ILog log)
        {
            _log = log;
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepo(builder);
            RegisterBlob(builder);
        }

        private void RegisterRepo(ContainerBuilder builder)
        {
            builder.RegisterInstance(new AssetRepository())
                .As<IAssetRepository>();

            builder.RegisterInstance(new OperationMetaRepository(
                AzureTableStorage<OperationMetaEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "OperationMeta", _log)))
                .As<IOperationMetaRepository>();

            builder.RegisterInstance(new OperationEventRepository(
                    AzureTableStorage<OperationEventTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "OperationEvents", _log)))
                .As<IOperationEventRepository>();


            builder.RegisterInstance(new UnconfirmedTransactionRepository(
                AzureTableStorage<UnconfirmedTransactionEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "UnconfirmedTransactions", _log)))
                .As<IUnconfirmedTransactionRepository>();

            builder.RegisterInstance(new ObservableOperationRepository(
                AzureTableStorage<ObservableOperationEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "ObservableOperations", _log)))
                .As<IObservableOperationRepository>();

            builder.RegisterInstance(new ObservableWalletRepository(
                AzureTableStorage<ObservableWalletEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "ObservableWallets", _log)))
                .As<IObservableWalletRepository>();

            builder.RegisterInstance(new WalletBalanceRepository(
                    AzureTableStorage<WalletBalanceEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "WalletBalances", _log)))
                .As<IWalletBalanceRepository>();
        }

        private void RegisterBlob(ContainerBuilder builder)
        {
            builder.RegisterInstance(
                new TransactionBlobStorage(AzureBlobStorage.Create(_settings.Nested(p => p.Db.DataConnString))))
                .As<ITransactionBlobStorage>();
        }
    }
}
