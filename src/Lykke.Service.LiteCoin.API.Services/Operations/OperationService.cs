using System;
using System.Linq;
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
        private readonly IQueueRouter<CashOutStartedNotificationContext> _cashoutStartedNotificationQueue;
        private readonly ICashOutOperationRepository _cashOutOperationRepository;
        private readonly IPendingCashoutTransactionRepository _pendingCashoutTransactionRepository;
        private readonly ILog _log;
        private readonly IBroadcastService _broadcastService;

        public OperationService(IWalletService walletService, 
            ITransactionBuilderService transactionBuilder, 
            ISignService signService,
            IQueueRouter<CashOutStartedNotificationContext> cashoutStartedNotificationQueue, 
            ICashOutOperationRepository cashOutOperationRepository,
            IPendingCashoutTransactionRepository pendingCashoutTransactionRepository,
            ILog log, 
            IBroadcastService broadcastService)
        {
            _walletService = walletService;
            _transactionBuilder = transactionBuilder;
            _signService = signService;
            _cashoutStartedNotificationQueue = cashoutStartedNotificationQueue;
            _cashOutOperationRepository = cashOutOperationRepository;
            _pendingCashoutTransactionRepository = pendingCashoutTransactionRepository;
            _log = log;
            _broadcastService = broadcastService;
        }

        public async Task<ICashOutOperation> ProceedCashOutOperation(string operationId, IWallet sourceWallet, BitcoinAddress destAddress, decimal amount)
        {
            var hotWallets = await _walletService.GetHotWallets();
            var assetId = Constants.AssetsContants.LiteCoin;

            
            foreach (var hotWallet in hotWallets)
            {
                try
                {
                    var unsignedTx = await _transactionBuilder.GetTransferTransaction(hotWallet.Address, 
                        destAddress,
                        amount);
                    
                    var signedtx = await _signService.SignTransaction(operationId, unsignedTx, hotWallet.Address);
                    
                    await _broadcastService.BroadCastTransaction(signedtx);

                    var operation = CashOutOperation.Create(operationId, sourceWallet.Address.ToString(),
                        destAddress.ToString(), amount, assetId, DateTime.UtcNow, signedtx.GetHash().ToString());

                    await _cashOutOperationRepository.Insert(operation);

                    await _pendingCashoutTransactionRepository.Insert(
                        CashOutTransaction.Create(signedtx.GetHash().ToString(), operationId));

                    await _cashoutStartedNotificationQueue.AddMessage(new CashOutStartedNotificationContext
                    {
                        OperationId = operationId
                    });

                    return operation;
                }
                catch (BusinessException e) when(e.Code == ErrorCode.NotEnoughFundsAvailable) //omit ex
                {

                }
            }

            await _log.WriteWarningAsync(nameof(OperationService), nameof(ProceedCashOutOperation), new
            {
                operationId,
                amount
            }.ToJson(), $"Not enough funds on hotwallets");

            throw new BusinessException("Not enoughFunds on Hot wallets", ErrorCode.NotEnoughFundsAvailable);
        }

        public async Task ProceedSendMoneyToHotWalletOperation(string operationId, IWallet sourceWallet, string thHash)
        {
            var hotWallet = (await _walletService.GetHotWallets()).First();

            var unsignedTx = await _transactionBuilder.GetSendMoneyToHotWalletTransaction(sourceWallet.Address, hotWallet.Address,
                thHash);
            var signedTx = await _signService.SignTransaction(operationId, unsignedTx, sourceWallet.Address);

            await _broadcastService.BroadCastTransaction(signedTx);
        }
    }
}
