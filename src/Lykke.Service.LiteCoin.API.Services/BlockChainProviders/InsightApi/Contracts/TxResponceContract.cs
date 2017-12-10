using Newtonsoft.Json;

namespace Lykke.Service.LiteCoin.API.Services.BlockChainProviders.InsightApi.Contracts
{
    public class TxResponceContract
    {
        [JsonProperty("confirmations")]
        public int Confirmation { get; set; }
    }
}
