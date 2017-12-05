namespace Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings
{
    public class LiteCoinAPISettings
    {
        public DbSettings Db { get; set; }
        public string Network { get; set; }
        public string InsightAPIUrl { get; set; }
    }
}
