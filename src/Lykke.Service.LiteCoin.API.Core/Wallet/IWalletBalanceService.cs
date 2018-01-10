using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Wallet
{
    public interface IWalletBalanceService
    {
        Task Subscribe(string address);
        Task Unsubscribe(string address);
        Task<IEnumerable<IWalletBalance>> GetPagedBalances(int skip, int take);
    }
}
