using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.Service.LiteCoin.API.Services.BlockChainReaders.InsightApi.Contracts
{
    public class RawTxResponce
    {
        [JsonProperty("rawtx")]
        public string RawTx { get; set; }
    }
}
