using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.Service.LiteCoin.API.Services.Operations;

namespace Lykke.Job.LiteCoin.Functions
{
    public class SendCashInToHotWalletFunctions
    {
        private readonly IWalletService _walletService;
        private readonly ITransactionOutputsService _outputsService;
        private readonly ICashInOperationRepository _cashInOperationRepository;
        private readonly ILog _log;
        private readonly ICashInEventRepository _cashInEventRepository;
        private readonly OperationsConfirmationsSettings _confirmationsSettings;
        private readonly IQueueRouter<SendCashInToHotWalletContext> _queueRouter;
        private readonly IOperationService _operationService;

        public SendCashInToHotWalletFunctions(IWalletService walletService,
            ITransactionOutputsService outputsService, 
            ICashInOperationRepository cashInOperationRepository,
            ILog log,
            ICashInEventRepository cashInEventRepository, 
            OperationsConfirmationsSettings confirmationsSettings, 
            IQueueRouter<SendCashInToHotWalletContext> queueRouter,
            IOperationService operationService)
        {
            _walletService = walletService;
            _outputsService = outputsService;
            _cashInOperationRepository = cashInOperationRepository;
            _log = log;
            _cashInEventRepository = cashInEventRepository;
            _confirmationsSettings = confirmationsSettings;
            _queueRouter = queueRouter;
            _operationService = operationService;
        }

        [QueueTrigger(SendCashInToHotWalletContext.QueueName)]
        public async Task Send(SendCashInToHotWalletContext context)
        {
            var operation = await _cashInOperationRepository.GetByOperationId(context.OperationId);
            if (operation == null)
            {
                await _log.WriteWarningAsync(nameof(SendCashInToHotWalletFunctions), nameof(Send), context.ToJson(),
                    "Operation not found");

                return;
            }

            var clientWallet = await _walletService.GetByPublicAddress(operation.DestinationAddress);

            if (clientWallet == null)
            {
                await _log.WriteWarningAsync(nameof(SendCashInToHotWalletFunctions), nameof(Send), context.ToJson(),
                    "Client wallet not found");

                return;
            }

            try
            {
                await _operationService.ProceedSendMoneyToHotWalletOperation(operation.OperationId, clientWallet,
                    operation.TxHash);
            }
            catch (BusinessException e) when(e.Code == ErrorCode.BalanceIsLessThanFee)
            {
                await _log.WriteWarningAsync(nameof(SendCashInToHotWalletFunctions), nameof(Send), context.ToJson(),
                    "Balance is less than fee");
            }


            await _cashInEventRepository.InsertEvent(CashInEvent.Create(operation.OperationId,
                CashInEventType.MoneyTransferredToHotWallet));
        }

        [TimerTrigger("05:00:00")]
        public async Task Retry()
        {
            var wallets = await _walletService.GetClientWallets();

            foreach (var wallet in wallets)
            {
                var outputs = await _outputsService.GetUnspentOutputs(wallet.Address.ToString(),
                    _confirmationsSettings.MinCashInRetryConfirmations);

                var txHashes = outputs.Select(p => p.Outpoint.Hash.ToString()).Distinct();

                foreach (var txHash in txHashes)
                {
                    var operation = await _cashInOperationRepository.GetByTxHash(txHash);

                    if (operation == null)
                    {
                        await _log.WriteWarningAsync(nameof(SendCashInToHotWalletFunctions), nameof(Retry), new
                        {
                            clientAddress = wallet.Address.ToString(),
                            txHash
                        }.ToJson(), "TxHash not found");
                    }
                    else
                    {
                        await _queueRouter.AddMessage(new SendCashInToHotWalletContext
                        {
                            OperationId = operation.OperationId
                        });

                        await _cashInEventRepository.InsertEvent(CashInEvent.Create(operation.OperationId,
                            CashInEventType.MoneyTransferredToHoRetry));

                        await _log.WriteWarningAsync(nameof(SendCashInToHotWalletFunctions), nameof(Retry), new
                        {
                            operation.OperationId
                        }.ToJson(), "Operation retry queued");
                    }
                }
            }
        }
    }
}
