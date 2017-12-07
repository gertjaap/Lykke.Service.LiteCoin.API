using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.Service.LiteCoin.API.Services.BlockChainReaders.InsightApi.Contracts
{
    internal class StatusResponceContract
    {
        [JsonProperty("info")]
        public InsightApiStatusInfoContract Info { get; set; }
    }

    internal class InsightApiStatusInfoContract
    {
        [JsonProperty("blocks")]
        public int LastBlockHeight { get; set; }
    }
}
