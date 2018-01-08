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
        private readonly IWalletBalanceRepository _balanceRepository;
        private readonly IBlockChainProvider _blockChainProvider;

        public UpdateBalanceFunctions(IWalletBalanceRepository balanceRepository, IObservableWalletRepository observableWalletRepository, IBlockChainProvider blockChainProvider)
        {
            _balanceRepository = balanceRepository;
            _observableWalletRepository = observableWalletRepository;
            _blockChainProvider = blockChainProvider;
        }

        [TimerTrigger("00:10:00")]
        public async Task UpdateBalances()
        {
            var wallets = (await _observableWalletRepository.GetAll()).ToList();

            foreach (var observableWallet in wallets)
            {
                var balance = await _blockChainProvider.GetBalanceSatoshi(observableWallet.Address);
                await _balanceRepository.InsertOrReplace(WalletBalance.Create(observableWallet.Address, balance));
            }
        }
    }
}
