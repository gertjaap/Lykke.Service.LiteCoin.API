using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashIn
{
    public interface ISettledCashInTransactionHandler
    {
        Task HandleSettledTransactions(IEnumerable<ICashInOperation> settledTransactions);
    }
}
