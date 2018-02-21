using Lykke.Service.Vertcoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.Vertcoin.API.Core.Settings.SlackNotifications;

namespace Lykke.Service.Vertcoin.API.Core.Settings
{
    public class AppSettings
    {
        public VertcoinApiSettings VertcoinAPI { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
