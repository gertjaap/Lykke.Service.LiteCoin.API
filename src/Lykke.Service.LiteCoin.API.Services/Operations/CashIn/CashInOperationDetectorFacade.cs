using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.Wallet;

namespace Lykke.Service.LiteCoin.API.Services.Operations.CashIn
{
    public class CashInOperationDetectorFacade: ICashInOperationDetectorFacade
    {
        private readonly ISettledCashInTransactionDetector _detector;
        private readonly OperationsConfirmationsSettings _operationsConfirmationsSettings;
        private readonly IWalletService _walletService;
        private readonly ISettledCashInTransactionHandler _settledTransactionsHandler;
        private readonly IDetectedAddressTransactionsRepository _detectedAddressTransactionsRepository;

        public CashInOperationDetectorFacade(ISettledCashInTransactionDetector detector, 
            OperationsConfirmationsSettings operationsConfirmationsSettings, 
            IWalletService walletService,
            ISettledCashInTransactionHandler settledTransactionsHandler,
            IDetectedAddressTransactionsRepository detectedAddressTransactionsRepository)
        {
            _detector = detector;
            _operationsConfirmationsSettings = operationsConfirmationsSettings;
            _walletService = walletService;
            _settledTransactionsHandler = settledTransactionsHandler;
            _detectedAddressTransactionsRepository = detectedAddressTransactionsRepository;
        }

        public async Task DetectCashInOps()
        {
            var wallets = await _walletService.GetClientWallets();

            foreach (var wallet in wallets)
            {
                var prevDetectedTransactions = await 
                    _detectedAddressTransactionsRepository.GetTxsForAddress(wallet.Address.ToString());


                var detectResult = await _detector.GetCashInOperations(wallet, prevDetectedTransactions,
                    _operationsConfirmationsSettings.MinCashInConfirmations);

                await _settledTransactionsHandler.HandleSettledTransactions(detectResult.DetectedOperations);

                await _detectedAddressTransactionsRepository.Insert(detectResult.AddressTransactions);
            }
        }
    }
}
