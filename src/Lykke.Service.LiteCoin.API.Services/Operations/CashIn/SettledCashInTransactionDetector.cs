using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.Constants;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Operations.CashIn
{
    public class DetectCashInOperationsResult : IDetectCashInOperationsResult
    {
        public IEnumerable<ICashInOperation> DetectedOperations { get; set; }

        public IEnumerable<IDetectedAddressTransaction> AddressTransactions { get; set; }

        public static DetectCashInOperationsResult Create(IEnumerable<ICashInOperation> cashInOperations,
            IEnumerable<IDetectedAddressTransaction> detectedTransactions)
        {
            return new DetectCashInOperationsResult
            {
                DetectedOperations = cashInOperations,
                AddressTransactions = detectedTransactions
            };
        }
    }

    public class SettledCashInTransactionDetector: ISettledCashInTransactionDetector
    {
        private readonly IBlockChainProvider _blockChainProvider;

        private readonly Network _network;
        private readonly ILog _log;

        public SettledCashInTransactionDetector(IBlockChainProvider blockChainProvider, 
            Network network, 
            ILog log)
        {
            _blockChainProvider = blockChainProvider;
            _network = network;
            _log = log;
        }
        

        private async Task<ICashInOperation> GetOperationFromTx(string txHash, IWallet wallet)
        {
            var tx = await _blockChainProvider.GetRawTx(txHash);
            
            var coins = tx.Outputs.AsCoins().Where(p => p.ScriptPubKey.GetDestinationAddress(_network) == wallet.Address).ToList();

            var amount = new Money(coins.Sum(p => p.Amount.Satoshi)).Satoshi;

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
                    operationId: Guid.NewGuid(), 
                    destinationAddress: wallet.Address.ToString(),
                    sourceAddress: sourceAddress,
                    txHash: txHash,
                    amount: amount,
                    assetId: Constants.AssetsContants.LiteCoin, 
                    detectedAt: DateTime.UtcNow);
            }

            return null;
        }

        public async Task<IDetectCashInOperationsResult> GetCashInOperations(IWallet wallet,
            IEnumerable<IDetectedAddressTransaction> prevDetectedTransactions,
            int minTxConfirmationCount)
        {
            var detectedTransactionsDictionary =
                prevDetectedTransactions.Select(p => p.TransactionHash).Distinct().ToDictionary(p => p);

            var txHashes = (await _blockChainProvider.GetTransactionsForAddress(wallet.Address)).Distinct().ToList();
            var newTransactions = txHashes.Where(p => !detectedTransactionsDictionary.ContainsKey(p)).ToList();


            var cashInOperations = new List<ICashInOperation>();
            var newDetectedAddressTransactions = new List<IDetectedAddressTransaction>();

            foreach (var txHash in newTransactions.Where(hash=> !detectedTransactionsDictionary.ContainsKey(hash)))
            {
                var confirmationCount = await _blockChainProvider.GetTxConfirmationCount(txHash);
                if (confirmationCount >= minTxConfirmationCount)
                {
                    newDetectedAddressTransactions.Add(DetectedAddressTransaction.Create(txHash, wallet.Address.ToString()));

                    var op = await GetOperationFromTx(txHash, wallet);

                    if (op != null)
                    {
                        await _log.WriteInfoAsync(nameof(SettledCashInTransactionDetector), nameof(GetCashInOperations),
                            op.ToJson(), "CashIn detected");

                        cashInOperations.Add(op);
                    }
                }

            }
            
            return DetectCashInOperationsResult.Create(cashInOperations, newDetectedAddressTransactions);
        }
    }
}
