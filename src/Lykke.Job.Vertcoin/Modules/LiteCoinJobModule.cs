using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.JobTriggers.Extenstions;
using Lykke.Service.Vertcoin.API.Core.Services;
using Lykke.Service.Vertcoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.Vertcoin.API.Services;
using Lykke.Service.Vertcoin.API.Services.Health;
using Lykke.Service.Vertcoin.API.Services.LifeiteManagers;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.Vertcoin.Modules
{
    public class VertcoinJobModule : Module
    {
        private readonly IReloadingManager<VertcoinApiSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public VertcoinJobModule(IReloadingManager<VertcoinApiSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterAzureQueueHandlers(builder);

            // TODO: Add your dependencies here

            builder.Populate(_services);
        }

        private void RegisterAzureQueueHandlers(ContainerBuilder builder)
        {
            builder.AddTriggers(
                pool =>
                {
                    pool.AddDefaultConnection(_settings.Nested(x=>x.Db.DataConnString));
                });
        }

    }
}
