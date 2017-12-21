using Newtonsoft.Json;

namespace Lykke.Service.LiteCoin.API.Services.BlockChainProviders.InsightApi.Contracts
{
    public class TxResponceContract
    {
        [JsonProperty("confirmations")]
        public int Confirmation { get; set; }

        [JsonProperty("vin")]
        public OutputContract[] Outputs { get; set; }

        public class OutputContract
        {
            [JsonProperty("n")]
            public uint N { get; set; }

            [JsonProperty("scriptPubKey")]
            public ScriptPubKeyContract ScriptPubKey { get; set; }
            public class ScriptPubKeyContract
            {
                [JsonProperty("addresses")]
                public string[] Addresses { get; set; }
            }
        }
    }




}
