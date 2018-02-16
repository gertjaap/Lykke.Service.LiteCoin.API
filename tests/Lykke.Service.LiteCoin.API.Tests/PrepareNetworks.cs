namespace Lykke.Service.LiteCoin.API.Tests
{
    public static class PrepareNetworks
    {
        static PrepareNetworks()
        {
            NBitcoin.Litecoin.Networks.EnsureRegistered();
        }

        public static void EnsureLiteCoinPrepared()
        {
            
        }
    }
}
