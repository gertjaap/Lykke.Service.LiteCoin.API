using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Pagination;

namespace Lykke.Service.LiteCoin.API.Core.Wallet
{

    public interface IWalletBalanceService
    {
        Task Subscribe(string address);
        Task Unsubscribe(string address);
        Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation);
        Task UpdateBalance(string address);
        Task UpdateBalance(IObservableWallet wallet);
    }
}
