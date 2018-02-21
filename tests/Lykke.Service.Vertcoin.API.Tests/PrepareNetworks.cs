namespace Lykke.Service.Vertcoin.API.Tests
{
    public static class PrepareNetworks
    {
        static PrepareNetworks()
        {
            NBitcoin.Vertcoin.Networks.EnsureRegistered();
        }

        public static void EnsureVertcoinPrepared()
        {
            
        }
    }
}
