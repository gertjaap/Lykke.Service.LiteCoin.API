using System;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Operations
{
    public class OperationService:IOperationService
    {
        private readonly ITransactionBuilderService _transactionBuilder;
        private readonly IOperationMetaRepository _operationMetaRepository;
        private readonly IOperationEventRepository _operationEventRepository;
        private readonly ISpentOutputService _spentOutputService;

        public OperationService(ITransactionBuilderService transactionBuilder,
            IOperationMetaRepository operationMetaRepository, 
            IOperationEventRepository operationEventRepository,
            ISpentOutputService spentOutputService)
        {
            _transactionBuilder = transactionBuilder;
            _operationMetaRepository = operationMetaRepository;
            _operationEventRepository = operationEventRepository;
            _spentOutputService = spentOutputService;
        }

        public async Task<Transaction> BuildTransferTransaction(Guid operationId,
            BitcoinAddress fromAddress, 
            BitcoinAddress toAddress,
            string assetId,
            Money amount, bool includeFee)
        {
            if (await _operationMetaRepository.Exist(operationId))
            {
                throw new BusinessException($"Operation {operationId} already exist", ErrorCode.BadInputParameter);
            }

            var operation = OperationMeta.Create(operationId, fromAddress.ToString(), toAddress.ToString(), assetId,
                amount.Satoshi, includeFee);

            await _operationMetaRepository.Insert(operation);

            var tx = await _transactionBuilder.GetTransferTransaction(fromAddress, toAddress, amount, includeFee);
            await _spentOutputService.SaveSpentOutputs(tx);

            await _operationEventRepository.InsertIfNotExist(OperationEvent.Create(operationId, OperationEventType.Builded));

            return tx;
        }
    }
}
