using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Services.Operations;

namespace Lykke.Job.LiteCoin.OperationsDetector.AzureQueueHandlers
{
    public class DetectCashOutFunctions
    {
        private readonly OperationsConfirmationsSettings _confirmationsSettings;
        private readonly ISettledCashoutTransactionHandler _settledCashoutTransactionHandler;
        private readonly ISettledCashOutTransactionDetector _cashOutTransactionDetector;
        private readonly IPendingCashoutTransactionRepository _pendingCashoutTransactionRepository;

        public DetectCashOutFunctions(OperationsConfirmationsSettings confirmationsSettings, 
            ISettledCashoutTransactionHandler settledCashoutTransactionHandler, 
            ISettledCashOutTransactionDetector cashOutTransactionDetector, 
            IPendingCashoutTransactionRepository pendingCashoutTransactionRepository)
        {
            _confirmationsSettings = confirmationsSettings;
            _settledCashoutTransactionHandler = settledCashoutTransactionHandler;
            _cashOutTransactionDetector = cashOutTransactionDetector;
            _pendingCashoutTransactionRepository = pendingCashoutTransactionRepository;
        }

        [TimerTrigger("00:10:00")]
        public async Task HandleCashOutCompleted()
        {
            var pendingTxs = await _pendingCashoutTransactionRepository.GetAll();

            var settledCashouts = await _cashOutTransactionDetector.CheckSettlement(pendingTxs, _confirmationsSettings.MinCashOutConfirmations);

            await _settledCashoutTransactionHandler.HandleSettledTransactions(settledCashouts);

        }
    }
}
