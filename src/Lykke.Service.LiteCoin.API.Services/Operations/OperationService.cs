using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.Broadcast;
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
        private readonly IQueueRouter<CashOutStartedNotificationContext> _cashoutCompletedNotificationQueue;
        private readonly ICashOutOperationRepository _cashOutOperationRepository;
        private readonly IPendingCashoutTransactionRepository _pendingCashoutTransactionRepository;
        private readonly ILog _log;
        private readonly IBroadcastService _broadcastService;

        public OperationService(IWalletService walletService, 
            ITransactionBuilderService transactionBuilder, 
            ISignService signService,
            IQueueRouter<CashOutStartedNotificationContext> cashoutCompletedNotificationQueue, 
            ICashOutOperationRepository cashOutOperationRepository,
            IPendingCashoutTransactionRepository pendingCashoutTransactionRepository,
            ILog log, 
            IBroadcastService broadcastService)
        {
            _walletService = walletService;
            _transactionBuilder = transactionBuilder;
            _signService = signService;
            _cashoutCompletedNotificationQueue = cashoutCompletedNotificationQueue;
            _cashOutOperationRepository = cashOutOperationRepository;
            _pendingCashoutTransactionRepository = pendingCashoutTransactionRepository;
            _log = log;
            _broadcastService = broadcastService;
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
                    
                    var signedtx = await _signService.SignTransaction(tx.Transaction, hotWallet.Address);
                    
                    await _broadcastService.BroadCastTransaction(signedtx);

                    await _cashOutOperationRepository.Insert(CashOutOperation.Create(operationId, sourceWallet.Address.ToString(),
                        destAddress.ToString(), amount, assetId, DateTime.UtcNow, tx.Transaction.GetHash().ToString()));

                    await _pendingCashoutTransactionRepository.Insert(
                        CashOutTransaction.Create(tx.Transaction.GetHash().ToString(), operationId));

                    await _cashoutCompletedNotificationQueue.AddMessage(new CashOutStartedNotificationContext
                    {
                        OperationId = operationId
                    });

                    return;
                }
                catch (BusinessException e) when(e.Code == ErrorCode.NotEnoughFundsAvailable)
                {
                    await _log.WriteInfoAsync(nameof(OperationService), nameof(ProceedCashOutOperation), new
                    {
                        operationId,
                        hotWallet
                    }.ToJson(), $"Not enough funds on hotwallet {hotWallet.Address}");
                }
            }
            
            throw new BusinessException("Not enoughFunds on Hot wallets", ErrorCode.NotEnoughFundsAvailable);
        }
    }
}
