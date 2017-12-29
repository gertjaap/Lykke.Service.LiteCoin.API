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
        private readonly ICashOutOperationRepository _cashOutOperationRepository;
        private readonly IPendingCashoutTransactionRepository _pendingCashoutTransactionRepository;
        private readonly ILog _log;
        private readonly IBroadcastService _broadcastService;
        private readonly IPendingCashOutEventRepository _cashOutNotificationRepository;

        public OperationService(IWalletService walletService, 
            ITransactionBuilderService transactionBuilder, 
            ISignService signService,
            ICashOutOperationRepository cashOutOperationRepository,
            IPendingCashoutTransactionRepository pendingCashoutTransactionRepository,
            ILog log, 
            IBroadcastService broadcastService,
            IPendingCashOutEventRepository cashOutNotificationRepository)
        {
            _walletService = walletService;
            _transactionBuilder = transactionBuilder;
            _signService = signService;
            _cashOutOperationRepository = cashOutOperationRepository;
            _pendingCashoutTransactionRepository = pendingCashoutTransactionRepository;
            _log = log;
            _broadcastService = broadcastService;
            _cashOutNotificationRepository = cashOutNotificationRepository;
        }

        public async Task<ICashOutOperation> ProceedCashOutOperation(Guid operationId, IWallet sourceWallet, BitcoinAddress destAddress, decimal amount)
        {
            var hotWallets = await _walletService.GetHotWallets();
            var assetId = Constants.AssetsContants.LiteCoin;
            
            foreach (var hotWallet in hotWallets)
            {
                Transaction unsignedTx;
                try
                {
                    unsignedTx = await _transactionBuilder.GetTransferTransaction(hotWallet.Address, 
                        destAddress,
                        amount);
                }
                catch (BusinessException e) when (e.Code == ErrorCode.NotEnoughFundsAvailable) //go to next hot wallet
                {
                    continue;
                }

                var signedtx = await _signService.SignTransaction(operationId, unsignedTx, hotWallet.Address);
                    
                await _broadcastService.BroadCastTransaction(signedtx);

                var operation = CashOutOperation.Create(operationId, sourceWallet.Address.ToString(),
                    destAddress.ToString(), amount, assetId, DateTime.UtcNow, signedtx.GetHash().ToString());

                await _cashOutOperationRepository.Insert(operation);

                await _cashOutNotificationRepository.Insert(
                    PendingCashOutEvent.Create(operation, PendingCashOutEventStatusType.Started));
                
                await _pendingCashoutTransactionRepository.InsertOrReplace(
                    CashOutTransaction.Create(signedtx.GetHash().ToString(), operationId));


                return operation;
            }

            await _log.WriteWarningAsync(nameof(OperationService), nameof(ProceedCashOutOperation), new
            {
                operationId,
                amount
            }.ToJson(), "Not enough funds on hot wallets");

            throw new BusinessException("Not enough funds on hot wallets", ErrorCode.NotEnoughFundsAvailable);
        }

        public async Task ProceedSendMoneyToHotWalletOperation(Guid operationId, IWallet sourceWallet, string thHash)
        {
            var hotWallet = (await _walletService.GetHotWallets()).First();

            var unsignedTx = await _transactionBuilder.GetSendMoneyToHotWalletTransaction(sourceWallet.Address, hotWallet.Address,
                thHash);
            var signedTx = await _signService.SignTransaction(operationId, unsignedTx, sourceWallet.Address);

            await _broadcastService.BroadCastTransaction(signedTx);
        }
    }
}
