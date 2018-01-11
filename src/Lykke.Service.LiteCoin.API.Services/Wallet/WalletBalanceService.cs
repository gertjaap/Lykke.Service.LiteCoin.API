using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Pagination;
using Lykke.Service.LiteCoin.API.Core.Wallet;

namespace Lykke.Service.LiteCoin.API.Services.Wallet
{
    public class WalletBalanceService:IWalletBalanceService
    {
        private readonly IWalletBalanceRepository _balanceRepository;
        private readonly IObservableWalletRepository _observableWalletRepository;

        public WalletBalanceService(IWalletBalanceRepository balanceRepository, IObservableWalletRepository observableWalletRepository)
        {
            _balanceRepository = balanceRepository;
            _observableWalletRepository = observableWalletRepository;
        }

        public async Task Subscribe(string address)
        {
            await _observableWalletRepository.Insert(ObservableWallet.Create(address));
        }

        public async Task Unsubscribe(string address)
        {
            await _observableWalletRepository.Delete(address);
            await _balanceRepository.DeleteIfExist(address);
        }

        public async Task<IPadedResult<IWalletBalance>> GetBalances(int take, string continuation)
        {
            //TODO add db pagination

            return PagedResult<WalletBalance>.Create(await _balanceRepository.GetAll(), continuation: null);
        }
    }
}
