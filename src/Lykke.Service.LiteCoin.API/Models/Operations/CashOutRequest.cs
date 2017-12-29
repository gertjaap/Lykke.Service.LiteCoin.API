using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Models.Operations
{
    public class CashOutRequest
    {
        public string SourceAddress { get; set; } 

        public string DestAddress { get; set; }

        public string AssetId { get; set; }

        public long Amount { get; set; }
    }
}
