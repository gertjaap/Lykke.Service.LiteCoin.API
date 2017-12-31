using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.CashOut;

namespace Lykke.Service.LiteCoin.API.Services.Operations.CashOut
{
    public class CashOutsOperationDetectorFacade: ICashOutsOperationDetectorFacade
    {
        private readonly OperationsConfirmationsSettings _confirmationsSettings;
        private readonly ISettledCashoutTransactionHandler _settledCashoutTransactionHandler;
        private readonly ISettledCashOutTransactionDetector _cashOutTransactionDetector;
        private readonly IUnconfirmedCashoutTransactionRepository _unconfirmedCashoutTransactionRepository;

        public CashOutsOperationDetectorFacade(OperationsConfirmationsSettings confirmationsSettings, 
            ISettledCashoutTransactionHandler settledCashoutTransactionHandler, 
            ISettledCashOutTransactionDetector cashOutTransactionDetector, 
            IUnconfirmedCashoutTransactionRepository unconfirmedCashoutTransactionRepository)
        {
            _confirmationsSettings = confirmationsSettings;
            _settledCashoutTransactionHandler = settledCashoutTransactionHandler;
            _cashOutTransactionDetector = cashOutTransactionDetector;
            _unconfirmedCashoutTransactionRepository = unconfirmedCashoutTransactionRepository;
        }
        
        public async Task DetectCashOutOps()
        {
            var pendingTxs = await _unconfirmedCashoutTransactionRepository.GetAll();

            var settledCashouts = await _cashOutTransactionDetector.CheckSettlement(pendingTxs, _confirmationsSettings.MinCashOutConfirmations);

            await _settledCashoutTransactionHandler.HandleSettledTransactions(settledCashouts);
        }
    }
}
