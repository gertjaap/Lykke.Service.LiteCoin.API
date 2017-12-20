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
        private readonly ITransactionBlobStorage _transactionBlobStorage;

        public OperationService(IWalletService walletService, 
            ITransactionBuilderService transactionBuilder, 
            ISignService signService,
            IBlockChainProvider blockChainProvider, 
            IQueueRouter<CashOutStartedNotificationContext> cashoutCompletedNotificationQueue, 
            ICashOutOperationRepository cashOutOperationRepository,
            IPendingCashoutTransactionRepository pendingCashoutTransactionRepository,
            ILog log, 
            ITransactionBlobStorage transactionBlobStorage)
        {
            _walletService = walletService;
            _transactionBuilder = transactionBuilder;
            _signService = signService;
            _blockChainProvider = blockChainProvider;
            _cashoutCompletedNotificationQueue = cashoutCompletedNotificationQueue;
            _cashOutOperationRepository = cashOutOperationRepository;
            _pendingCashoutTransactionRepository = pendingCashoutTransactionRepository;
            _log = log;
            _transactionBlobStorage = transactionBlobStorage;
        }

        public async Task ProceedCashOutOperation(string operationId, IWallet sourceWallet, BitcoinAddress destAddress, decimal amount)
        {
            var hotWallets = await _walletService.GetHotWallets();
            var assetId = Constants.AssetsContants.LiteCoin;

            foreach (var hotWallet in hotWallets)
            {
                try
                {
                    var tx = await _transactionBuilder.GetTransferTransaction(hotWallet.Address, 
                        destAddress,
                        amount);

                    await _transactionBlobStorage.AddOrReplaceTransaction(operationId, TransactionBlobType.Initial, tx.Transaction.ToHex());

                    await _signService.SignTransaction(tx.Transaction, hotWallet.WalletId);

                    await _transactionBlobStorage.AddOrReplaceTransaction(operationId, TransactionBlobType.Signed, tx.Transaction.ToHex());

                    await _blockChainProvider.BroadCastTransaction(tx.Transaction);

                    await _cashOutOperationRepository.Insert(CashOutOperation.Create(operationId, sourceWallet.WalletId,
                        destAddress.ToString(), amount, assetId, DateTime.UtcNow, tx.Transaction.GetHash().ToString()));

                    await _pendingCashoutTransactionRepository.Insert(
                        CashOutTransaction.Create(tx.Transaction.GetHash().ToString(), operationId));

                    await _cashoutCompletedNotificationQueue.AddMessage(new CashOutStartedNotificationContext
                    {
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
