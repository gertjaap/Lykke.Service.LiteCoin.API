using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Wallet;

namespace Lykke.Service.LiteCoin.API.Core.CashIn
{

    public interface ISettledCashInTransactionDetector
    {
        Task<IEnumerable<ICashInOperation>> GetCashInOperations(IEnumerable<IWallet> wallets, int fromHeight, int toHeight);
    }
}
