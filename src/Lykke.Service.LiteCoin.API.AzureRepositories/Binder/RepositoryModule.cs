using Autofac;
using AzureStorage.Blob;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.LiteCoin.API.AzureRepositories.Asset;
using Lykke.Service.LiteCoin.API.AzureRepositories.Operations;
using Lykke.Service.LiteCoin.API.AzureRepositories.Queue;
using Lykke.Service.LiteCoin.API.AzureRepositories.TransactionOutput.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.AzureRepositories.TransactionOutput.SpentOutputs;
using Lykke.Service.LiteCoin.API.AzureRepositories.Transactions;
using Lykke.Service.LiteCoin.API.AzureRepositories.Wallet;
using Lykke.Service.LiteCoin.API.Core.Asset;
using Lykke.Service.LiteCoin.API.Core.ObservableOperation;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.SettingsReader;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Binder
{
    public  class RepositoryModule:Module
    {
        private readonly ILog _log;
        private readonly IReloadingManager<LiteCoinAPISettings> _settings;
        public RepositoryModule(IReloadingManager<LiteCoinAPISettings> settings, ILog log)
        {
            _log = log;
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepo(builder);
            RegisterQueue(builder);
            RegisterBlob(builder);
        }

        private void RegisterRepo(ContainerBuilder builder)
        {
            builder.RegisterInstance(new BroadcastedOutputRepository(
                    AzureTableStorage<BroadcastedOutputTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "BroadcastedOutputs", _log)))
                .As<IBroadcastedOutputRepository>();

            builder.RegisterInstance(new SpentOutputRepository(
                    AzureTableStorage<SpentOutputTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "SpentOutputs", _log)))
                .As<ISpentOutputRepository>();

            builder.RegisterInstance(new AssetRepository())
                .As<IAssetRepository>();

            builder.RegisterInstance(new OperationMetaRepository(
                AzureTableStorage<OperationMetaEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "OperationMeta", _log)))
                .As<IOperationMetaRepository>();

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

        private void RegisterQueue(ContainerBuilder builder)
        {
            builder.RegisterInstance(new AzureQueueFactory(_settings.Nested(p => p.Db.DataConnString)))
                .As<IQueueFactory>();

            builder.RegisterGeneric(typeof(QueueRouter<>)).As(typeof(IQueueRouter<>)).InstancePerDependency();
        }

        private void RegisterBlob(ContainerBuilder builder)
        {
            builder.RegisterInstance(
                new TransactionBlobStorage(AzureBlobStorage.Create(_settings.Nested(p => p.Db.DataConnString))))
                .As<ITransactionBlobStorage>();
        }
    }
}
