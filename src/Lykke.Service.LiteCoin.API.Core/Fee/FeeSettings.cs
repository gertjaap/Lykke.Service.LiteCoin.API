using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Core.Fee
{
    public class FeeSettings
    {
        public int DefaultFeePerKyloByte { get; set; } 
        
        public long MinFeeValueSatoshi { get; set; }

        public long MaxFeeValueSatoshi { get; set; }

        public string DynamicFeeProviderUrl { get; set; }
    }
}
