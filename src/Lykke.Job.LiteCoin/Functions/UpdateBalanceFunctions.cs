using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.Wallet;

namespace Lykke.Job.LiteCoin.Functions
{
    public class UpdateBalanceFunctions
    {
        private readonly IObservableWalletRepository _observableWalletRepository;
        private readonly IWalletBalanceService _walletBalanceService;

        public UpdateBalanceFunctions(IObservableWalletRepository observableWalletRepository, IWalletBalanceService walletBalanceService)
        {
            _observableWalletRepository = observableWalletRepository;
            _walletBalanceService = walletBalanceService;
        }

        [TimerTrigger("00:10:00")]
        public async Task UpdateBalances()
        {
            var wallets = (await _observableWalletRepository.GetAll()).ToList();

            foreach (var observableWallet in wallets)
            {
                await _walletBalanceService.UpdateBalance(observableWallet);
            }
        }
    }
}
