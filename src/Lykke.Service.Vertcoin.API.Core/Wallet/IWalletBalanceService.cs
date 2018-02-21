using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Vertcoin.API.Core.Pagination;

namespace Lykke.Service.Vertcoin.API.Core.Wallet
{

    public interface IWalletBalanceService
    {
        Task Subscribe(string address);
        Task Unsubscribe(string address);
        Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation);
        Task<IWalletBalance> UpdateBalance(string address);
        Task<IWalletBalance> UpdateBalance(IObservableWallet wallet);
    }
}
