using Newtonsoft.Json;

namespace Lykke.Service.LiteCoin.API.Services.BlockChainProviders.InsightApi.Contracts
{
    public class RawTxResponce
    {
        [JsonProperty("rawtx")]
        public string RawTx { get; set; }
    }
}
