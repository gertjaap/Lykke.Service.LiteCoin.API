using System;
using System.Threading.Tasks;
using Common.Log;
using Flurl.Http;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.Broadcast;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.ObservableOperation;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Broadcast
{
    public class BroadcastService:IBroadcastService
    {
        private readonly IBlockChainProvider _blockChainProvider;
        private readonly ILog _log;
        private readonly IUnconfirmedTransactionRepository _unconfirmedTransactionRepository;
        private readonly IBroadcastedOutputsService _broadcastedOutputsService;
        private readonly IOperationMetaRepository _operationMetaRepository;
        private readonly IOperationEventRepository _operationEventRepository;
        private readonly IObservableOperationRepository _observableOperationRepository;
        private readonly ITransactionBlobStorage _transactionBlobStorage;
        private readonly ISpentOutputService _spentOutputService;

        public BroadcastService(IBlockChainProvider blockChainProvider,
            ILog log, 
            IUnconfirmedTransactionRepository unconfirmedTransactionRepository,
            IBroadcastedOutputsService broadcastedOutputsService, 
            IOperationMetaRepository operationMetaRepository,
            IOperationEventRepository operationEventRepository,
            IObservableOperationRepository observableOperationRepository, 
            ITransactionBlobStorage transactionBlobStorage, 
            ISpentOutputService spentOutputService)
        {
            _blockChainProvider = blockChainProvider;
            _log = log;
            _unconfirmedTransactionRepository = unconfirmedTransactionRepository;
            _broadcastedOutputsService = broadcastedOutputsService;
            _operationMetaRepository = operationMetaRepository;
            _operationEventRepository = operationEventRepository;
            _observableOperationRepository = observableOperationRepository;
            _transactionBlobStorage = transactionBlobStorage;
            _spentOutputService = spentOutputService;
        }

        public async Task BroadCastTransaction(Guid operationId, Transaction tx)
        {
            var operation = await _operationMetaRepository.Get(operationId);
            if (operation == null)
            {
                throw new BusinessException("Operation not found", ErrorCode.BadInputParameter);
            }

            if (await _operationEventRepository.Exist(operationId, OperationEventType.Broadcasted))
            {
                throw new BusinessException("Operation not found", ErrorCode.TransactionAlreadyBroadcasted);
            }

            await _transactionBlobStorage.AddOrReplaceTransaction(operationId,TransactionBlobType.BeforeBroadcast, tx.ToHex());

            await _blockChainProvider.BroadCastTransaction(tx);

            await _broadcastedOutputsService.SaveNewOutputs(tx);
            await _spentOutputService.SaveSpentOutputs(tx);
            
            await _operationEventRepository.InsertIfNotExist(OperationEvent.Create(operationId, OperationEventType.Broadcasted));

            await _observableOperationRepository.InsertOrReplace(ObervableOperation.Create(operation,
                BroadcastStatus.InProgress, tx.GetHash().ToString()));

            await _unconfirmedTransactionRepository.InsertOrReplace(
                UnconfirmedTransaction.Create(operationId, tx.GetHash().ToString()));
        }

        public async Task BroadCastTransaction(Guid operationId, string txHex)
        {
            Transaction tx;

            try
            {
                tx = Transaction.Parse(txHex);
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync(nameof(BroadcastService), nameof(BroadCastTransaction),
                    txHex, e);
                throw new BusinessException("Invalid transaction hex", ErrorCode.BadInputParameter);
            }

            await BroadCastTransaction(operationId, tx);
        }
    }
}
