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
        private readonly ISpentOutputService _spentOutputService;

        public OperationService(ITransactionBuilderService transactionBuilder,
            IOperationMetaRepository operationMetaRepository, 
            ISpentOutputService spentOutputService)
        {
            _transactionBuilder = transactionBuilder;
            _operationMetaRepository = operationMetaRepository;
            _spentOutputService = spentOutputService;
        }

        public async Task<Transaction> BuildTransferTransaction(Guid operationId,
            BitcoinAddress fromAddress, 
            BitcoinAddress toAddress,
            string assetId,
            Money amountToSend, bool includeFee)
        {
            if (await _operationMetaRepository.Exist(operationId))
            {
                throw new BusinessException($"Operation {operationId} already exist", ErrorCode.BadInputParameter);
            }


            var buildedTransaction = await _transactionBuilder.GetTransferTransaction(fromAddress, toAddress, amountToSend, includeFee);

            var operation = OperationMeta.Create(operationId, fromAddress.ToString(), toAddress.ToString(), assetId,
                buildedTransaction.Amount.Satoshi, buildedTransaction.Fee.Satoshi, includeFee);
            await _operationMetaRepository.Insert(operation);

            await _spentOutputService.SaveSpentOutputs(buildedTransaction.TransactionData);

            return buildedTransaction.TransactionData;
        }
    }
}
