using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashOut
{

    public interface ISettledCashOutTransactionDetector
    {
        Task<IEnumerable<IUnconfirmedCashoutTransaction>> CheckSettlement(IEnumerable<IUnconfirmedCashoutTransaction> trackedTransactions, int minConfirmationsCount);
    }
}
