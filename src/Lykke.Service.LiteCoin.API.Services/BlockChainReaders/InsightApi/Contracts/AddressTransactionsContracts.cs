using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.Service.LiteCoin.API.Services.BlockChainReaders.InsightApi.Contracts
{
    internal class AddressBalanceResponceContract
    {
        [JsonProperty("transactions")]
        public string[] Transactions { get; set; }
    }
}
