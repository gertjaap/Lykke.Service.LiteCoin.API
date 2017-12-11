using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.CashOut;

namespace Lykke.Service.LiteCoin.API.Services.CashOut
{

    public class CashOutSettlementDetector: ICashOutSettlementDetector
    {
        private readonly IBlockChainProvider _blockChainProvider;

        public CashOutSettlementDetector(IBlockChainProvider blockChainProvider)
        {
            _blockChainProvider = blockChainProvider;
        }


        public async Task<IEnumerable<ICashoutTransaction>> CheckSettlement(IEnumerable<ICashoutTransaction> trackedTransactions, int minConfirmationsCount)
        {
            var settledTransactions = new List<ICashoutTransaction>();

            foreach (var tx in trackedTransactions)
            {
                var confirmationCount = await _blockChainProvider.GetTxConfirmationCount(tx.TxHash);

                if (confirmationCount >= minConfirmationsCount)
                {
                    settledTransactions.Add(tx);
                }
            }

            return settledTransactions;
        }
    }
}
