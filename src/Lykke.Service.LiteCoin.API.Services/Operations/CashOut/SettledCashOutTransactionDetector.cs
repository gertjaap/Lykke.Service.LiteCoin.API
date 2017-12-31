using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.CashOut;

namespace Lykke.Service.LiteCoin.API.Services.Operations.CashOut
{

    public class SettledCashOutTransactionDetector: ISettledCashOutTransactionDetector
    {
        private readonly IBlockChainProvider _blockChainProvider;
        private readonly ILog _log;

        public SettledCashOutTransactionDetector(IBlockChainProvider blockChainProvider, 
            ILog log)
        {
            _blockChainProvider = blockChainProvider;
            _log = log;
        }


        public async Task<IEnumerable<IUnconfirmedCashoutTransaction>> CheckSettlement(IEnumerable<IUnconfirmedCashoutTransaction> trackedTransactions, int minConfirmationsCount)
        {
            var settledTransactions = new List<IUnconfirmedCashoutTransaction>();

            foreach (var tx in trackedTransactions)
            {
                var confirmationCount = await _blockChainProvider.GetTxConfirmationCount(tx.TxHash);

                if (confirmationCount >= minConfirmationsCount)
                {
                    await _log.WriteInfoAsync(nameof(SettledCashOutTransactionDetector), nameof(CheckSettlement),
                        tx.ToJson(), "Cashout settlement detected");

                    settledTransactions.Add(tx);
                }
            }

            return settledTransactions;
        }
    }
}
