using Autofac;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core;
using Lykke.Service.LiteCoin.API.Core.Address;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Fee;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.Core.SourceWallets;
using Lykke.Service.LiteCoin.API.Core.WebHook;
using Lykke.Service.LiteCoin.API.Services.Address;
using Lykke.Service.LiteCoin.API.Services.BlockChainProviders.InsightApi;
using Lykke.Service.LiteCoin.API.Services.CashOut;
using Lykke.Service.LiteCoin.API.Services.Fee;
using Lykke.Service.LiteCoin.API.Services.SignFacade;
using Lykke.Service.LiteCoin.API.Services.SourceWallet;
using Lykke.Service.LiteCoin.API.Services.WebHook;
using Lykke.SettingsReader;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Binder
{
    public  class ServiceModule:Module
    {
        private readonly ILog _log;
        private readonly IReloadingManager<LiteCoinAPISettings> _settings;
        public ServiceModule(IReloadingManager<LiteCoinAPISettings> settings, ILog log)
        {
            _log = log;
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterNetwork(builder);
            RegisterFeeServices(builder);
            RegisterAddressValidatorServices(builder);
            RegisterInsightApiBlockChainReaders(builder);
            RegisterWebHookServices(builder);
            RegisterSignFacadeServices(builder);
            RegisterCashOutServices(builder);
        }

        private void RegisterNetwork(ContainerBuilder builder)
        {
            NBitcoin.Litecoin.Networks.Register();
            builder.RegisterInstance(Network.GetNetwork(_settings.CurrentValue.Network)).As<Network>();
        }

        private void RegisterFeeServices(ContainerBuilder builder)
        {
            builder.RegisterInstance(new FeeRateFacade(_settings.CurrentValue.FeePerByte, _settings.CurrentValue.FeeRateMultiplayer)).As<IFeeRateFacade>();
            builder.Register(x =>
            {
                var resolver = x.Resolve<IComponentContext>();
                return new FeeFacade(resolver.Resolve<IFeeRateFacade>(), _settings.CurrentValue.MinFeeValue, _settings.CurrentValue.MaxFeeValue);
            });
        }

        private void RegisterAddressValidatorServices(ContainerBuilder builder)
        {
            builder.RegisterType<AddressValidator>().As<IAddressValidator>();
        }

        private void RegisterInsightApiBlockChainReaders(ContainerBuilder builder)
        {
            builder.RegisterInstance(new InsightApiSettings
            {
                Url = _settings.CurrentValue.InsightAPIUrl
            }).SingleInstance();

            builder.RegisterType<InsightApiBlockChainProvider>().As<IBlockChainProvider>();
        }

        private void RegisterWebHookServices(ContainerBuilder builder)
        {
            builder.RegisterInstance(new WebHookSettings
            {
                Url = _settings.CurrentValue.EventsWebHook
            }).SingleInstance();

            builder.RegisterType<WebHookSender>().As<IWebHookSender>();
        }

        private void RegisterSignFacadeServices(ContainerBuilder builder)
        {
            builder.RegisterInstance(new SourceWalletsSettings
            {
                SourceWalletIds = _settings.CurrentValue.SourceWallets
            }).SingleInstance();

            builder.RegisterInstance(new SignFacadeSettings
            {
                Url = _settings.CurrentValue.SignFacadeUrl
            }).SingleInstance();
            
            builder.RegisterType<SignFacadeService>().As<ISignFacadeService>().SingleInstance();
        }

        private void RegisterCashOutServices(ContainerBuilder builder)
        {

            builder.RegisterType<CashOutSettlementDetector>().As<ICashOutSettlementDetector>().SingleInstance();
        }
    }
}
