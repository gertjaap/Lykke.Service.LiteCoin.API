using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.BlockChainTracker;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.Constants;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Operations.CashIn
{
    public class SettledCashInTransactionDetector: ISettledCashInTransactionDetector
    {
        private readonly IBlockChainProvider _blockChainProvider;

        private readonly Network _network;

        public SettledCashInTransactionDetector(IBlockChainProvider blockChainProvider, 
            Network network)
        {
            _blockChainProvider = blockChainProvider;
            _network = network;
        }

        public async Task<IEnumerable<ICashInOperation>> GetCashInOperations(IEnumerable<IWallet> wallets,
            int fromHeight,
            int toHeight)
        {
            var result = new List<ICashInOperation>();

            foreach (var wallet in wallets)
            {
                var operationsForWallet = await GetCashInOperations(wallet, fromHeight, toHeight);

                result.AddRange(operationsForWallet);
            }


            return result;
        }

        private async Task<IEnumerable<ICashInOperation>> GetCashInOperations(IWallet wallet, 
            int fromHeight,
            int toHeight)
        {
            var txHashes = await _blockChainProvider.GetTransactionsForAddress(wallet.Address, fromHeight, toHeight);


            var result = new List<ICashInOperation>();

            foreach (var txHash in txHashes)
            {
                var op = await GetOperationFromTx(txHash, wallet);


                if (op != null)
                {
                    result.Add(op);
                }
            }


            return result;
        }

        private async Task<ICashInOperation> GetOperationFromTx(string txHash, IWallet wallet)
        {
            var tx = await _blockChainProvider.GetRawTx(txHash);
            
            var coins = tx.Outputs.AsCoins().Where(p => p.ScriptPubKey.GetDestinationAddress(_network) == wallet.Address).ToList();

            var amount = new Money(coins.Sum(p => p.Amount.Satoshi)).ToUnit(MoneyUnit.BTC);

            if (amount != 0)
            {
                string sourceAddress = null;
                foreach (var txInput in tx.Inputs)
                {
                    var prevTx = await _blockChainProvider.GetRawTx(txInput.PrevOut.Hash.ToString());
                    if (prevTx == null)
                    {
                        continue;
                    }

                    sourceAddress = await _blockChainProvider.GetDestinationAddress(txInput.PrevOut.Hash.ToString(), txInput.PrevOut.N); // Nbitcoin library function GetDestitionAddress  returns not valid address

                    if (sourceAddress != null)
                    {
                        break;
                    }
                }
            
                return CashInOperation.Create(
                    operationId: Guid.NewGuid().ToString("N"), 
                    destinationAddress: wallet.Address.ToString(),
                    sourceAddress: sourceAddress,
                    txHash: txHash,
                    amount: amount,
                    assetId: Constants.AssetsContants.LiteCoin, 
                    detectedAt: DateTime.UtcNow);
            }

            return null;
        }
    }
}
