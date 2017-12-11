using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Services.Operations
{
    public class OperationsConfirmationsSettings
    {
        public int MinCashOutConfirmations { get; set; }
        public int MinCashInConfirmations { get; set; }
    }
}
