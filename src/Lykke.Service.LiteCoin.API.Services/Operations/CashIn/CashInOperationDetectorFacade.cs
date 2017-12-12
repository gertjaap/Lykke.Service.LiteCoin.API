using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.BlockChainTracker;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.Wallet;

namespace Lykke.Service.LiteCoin.API.Services.Operations.CashIn
{
    public class CashInOperationDetectorFacade: ICashInOperationDetectorFacade
    {
        private readonly IBlockChainProvider _blockChainProvider;
        private readonly ISettledCashInTransactionDetector _detector;
        private readonly OperationsConfirmationsSettings _operationsConfirmationsSettings;
        private readonly ICashInLastProcessedBlockRepository _lastProcessedBlockRepository;
        private readonly IWalletService _walletService;
        private readonly ISettledCashInTransactionHandler _settledTransactionsHandler;

        public CashInOperationDetectorFacade(ISettledCashInTransactionDetector detector, 
            OperationsConfirmationsSettings operationsConfirmationsSettings, 
            ICashInLastProcessedBlockRepository lastProcessedBlockRepository, 
            IBlockChainProvider blockChainProvider, IWalletService walletService,
            ISettledCashInTransactionHandler settledTransactionsHandler)
        {
            _detector = detector;
            _operationsConfirmationsSettings = operationsConfirmationsSettings;
            _lastProcessedBlockRepository = lastProcessedBlockRepository;
            _blockChainProvider = blockChainProvider;
            _walletService = walletService;
            _settledTransactionsHandler = settledTransactionsHandler;
        }

        public async Task DetectCashInOps()
        {
            var lastProcessedBlock = await _lastProcessedBlockRepository.GetLastProcessedBlockHeight();
            var lastBlockChainBlock = await _blockChainProvider.GetLastBlockHeight();

            var fromHeight = lastProcessedBlock;
            var toHeight = lastBlockChainBlock - _operationsConfirmationsSettings.MinCashOutConfirmations;

            if (toHeight - fromHeight == 0)
            {
                return;
            }

            var wallets = await _walletService.GetClientWallets();

            var operations = await _detector.GetCashInOperations(wallets, fromHeight, toHeight);
            await _settledTransactionsHandler.HandleSettledTransactions(operations);

            await _lastProcessedBlockRepository.SetLastProcessedBlockHeight(toHeight);
        }
    }
}
