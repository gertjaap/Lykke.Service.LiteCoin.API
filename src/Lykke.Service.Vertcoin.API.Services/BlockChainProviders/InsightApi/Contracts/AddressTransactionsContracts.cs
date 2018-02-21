using Newtonsoft.Json;

namespace Lykke.Service.Vertcoin.API.Services.BlockChainProviders.InsightApi.Contracts
{
    internal class AddressBalanceResponceContract
    {
        [JsonProperty("transactions")]
        public string[] Transactions { get; set; }

        [JsonProperty("balanceSat")]
        public long BalanceSatoshi { get; set; }
    }
}
