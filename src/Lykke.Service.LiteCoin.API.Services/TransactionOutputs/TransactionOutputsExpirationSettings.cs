using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Services.TransactionOutputs
{
    public class TransactionOutputsExpirationSettings
    {

        public int BroadcastedOutputsExpirationDays { get; set; } 


        public int SpentOutputsExpirationDays { get; set; } 
    }
}
