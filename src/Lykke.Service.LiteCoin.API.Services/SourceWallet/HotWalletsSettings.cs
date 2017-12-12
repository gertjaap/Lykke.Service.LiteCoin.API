using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Services.SourceWallet
{
    public class HotWalletsSettings
    {
        public IEnumerable<string> SourceWalletIds { get; set; }
    }
}
