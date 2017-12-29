using Autofac;
using AzureStorage.Blob;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.LiteCoin.API.AzureRepositories.Asset;
using Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashIn;
using Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashOut;
using Lykke.Service.LiteCoin.API.AzureRepositories.Queue;
using Lykke.Service.LiteCoin.API.AzureRepositories.TransactionOutput.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.AzureRepositories.TransactionOutput.SpentOutputs;
using Lykke.Service.LiteCoin.API.AzureRepositories.Transactions;
using Lykke.Service.LiteCoin.API.Core.Asset;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using Lykke.Service.LiteCoin.API.Core.Transactions;
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
            builder.RegisterInstance(new PendingCashoutTransactionRepository(
                AzureTableStorage<PendingCashoutTransactionEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "PengingCashoutTransactions", _log)))
                    .As<IPendingCashoutTransactionRepository>();

            builder.RegisterInstance(new CashOutOperationRepository(
                AzureTableStorage<CashOutOperationTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "CashOutOperations", _log)))
                    .As<ICashOutOperationRepository>();

            builder.RegisterInstance(new CashOutEventRepository(
                    AzureTableStorage<CashOutEventTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "CashOutEvents", _log)))
                .As<ICashOutEventRepository>();


            builder.RegisterInstance(new CashInOperationRepository(
                    AzureTableStorage<CashInOperationEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "CashInOperations", _log)))
                .As<ICashInOperationRepository>();

            builder.RegisterInstance(new CashInEventRepository(
                    AzureTableStorage<CashInEventTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "CashInEvents", _log)))
                .As<ICashInEventRepository>();

            builder.RegisterInstance(new BroadcastedOutputRepository(
                    AzureTableStorage<BroadcastedOutputTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "BroadcastedOutputs", _log)))
                .As<IBroadcastedOutputRepository>();

            builder.RegisterInstance(new SpentOutputRepository(
                    AzureTableStorage<SpentOutputTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "SpentOutputs", _log)))
                .As<ISpentOutputRepository>();

            builder.RegisterInstance(new DetectedAddressTransactionsRepository(
                    AzureTableStorage<DetectedAddressTransactionEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "DetectedAddressTransactions", _log)))
                .As<IDetectedAddressTransactionsRepository>();



            builder.RegisterInstance(new PendingCashInNotificationRepository(
                    AzureTableStorage<PendingCashInNotificationTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "PendingCashInNotifications", _log)))
                .As<IPendingCashInNotificationRepository>();

            builder.RegisterInstance(new PendingCashOutNotificationRepository(
                    AzureTableStorage<PendingCashOutNotificationTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "PendingCashOutNotifications", _log)))
                .As<IPendingCashOutNotificationRepository>();


            builder.RegisterInstance(new AssetRepository())
                .As<IAssetRepository>();
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
