using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Constants;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Core.Sign;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Operations
{
    public class OperationService:IOperationService
    {
        private readonly IWalletService _walletService;
        private readonly ITransactionBuilderService _transactionBuilder;
        private readonly ISignService _signService;
        private readonly IBlockChainProvider _blockChainProvider;
        private readonly IQueueRouter<CashOutStartedNotificationContext> _cashoutCompletedNotificationQueue;
        private readonly ICashOutOperationRepository _cashOutOperationRepository;
        private readonly IPendingCashoutTransactionRepository _pendingCashoutTransactionRepository;
        private readonly ILog _log;

        public OperationService(IWalletService walletService, 
            ITransactionBuilderService transactionBuilder, 
            ISignService signService,
            IBlockChainProvider blockChainProvider, 
            IQueueRouter<CashOutStartedNotificationContext> cashoutCompletedNotificationQueue, 
            ICashOutOperationRepository cashOutOperationRepository,
            IPendingCashoutTransactionRepository pendingCashoutTransactionRepository,
            ILog log)
        {
            _walletService = walletService;
            _transactionBuilder = transactionBuilder;
            _signService = signService;
            _blockChainProvider = blockChainProvider;
            _cashoutCompletedNotificationQueue = cashoutCompletedNotificationQueue;
            _cashOutOperationRepository = cashOutOperationRepository;
            _pendingCashoutTransactionRepository = pendingCashoutTransactionRepository;
            _log = log;
        }

        public async Task ProceedCashOutOperation(string operationId, IWallet sourceWallet, BitcoinAddress destAddress, decimal amount)
        {
            var hotWallets = await _walletService.GetHotWallets();
            var assetId = Constants.AssetsContants.LiteCoin;

            foreach (var hotWallet in hotWallets)
            {
                try
                {
                    var hotWalletAddr = BitcoinAddress.Create(hotWallet.Address);

                    var tx = await _transactionBuilder.GetTransferTransaction(hotWalletAddr, 
                        destAddress,
                        amount);

                    await _signService.SignTransaction(tx.Transaction, hotWalletAddr);

                    await _blockChainProvider.BroadCastTransaction(tx.Transaction);

                    await _cashOutOperationRepository.Insert(CashOutOperation.Create(operationId, sourceWallet.WalletId,
                        destAddress.ToString(), amount, assetId, DateTime.UtcNow, tx.Transaction.GetHash().ToString()));

                    await _pendingCashoutTransactionRepository.Insert(
                        CashOutTransaction.Create(tx.Transaction.GetHash().ToString(), operationId));

                    await _cashoutCompletedNotificationQueue.AddMessage(new CashOutStartedNotificationContext
                    {
                        Amount = amount,
                        AssetId = assetId,
                        DateTime = DateTime.UtcNow,
                        DestAddress = destAddress.ToString(),
                        TxHash = tx.Transaction.GetHash().ToString(),
                        WalletId = sourceWallet.WalletId,
                        OperationId = operationId
                    });

                    return;
                }
                catch (BackendException e) when(e.Code == ErrorCode.NotEnoughFundsAvailable)
                {
                    await _log.WriteInfoAsync(nameof(OperationService), nameof(ProceedCashOutOperation), new
                    {
                        operationId,
                        hotWallet
                    }.ToJson(), $"Not enough funds on hotwallet {hotWallet.Address}");
                }
            }
            
            throw new BackendException("Not enoughFunds on Hot wallets", ErrorCode.NotEnoughFundsAvailable);
        }
    }
}
