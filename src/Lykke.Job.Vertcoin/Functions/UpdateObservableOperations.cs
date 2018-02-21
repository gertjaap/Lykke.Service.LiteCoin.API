﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.Vertcoin.API.Core.BlockChainReaders;
using Lykke.Service.Vertcoin.API.Core.ObservableOperation;
using Lykke.Service.Vertcoin.API.Core.Operation;
using Lykke.Service.Vertcoin.API.Core.Transactions;
using Lykke.Service.Vertcoin.API.Core.Wallet;
using Lykke.Service.Vertcoin.API.Services.Operations;

namespace Lykke.Job.Vertcoin.Functions
{
    public class UpdateObservableOperations
    {
        private readonly IUnconfirmedTransactionRepository _unconfirmedTransactionRepository;
        private readonly IBlockChainProvider _blockChainProvider;
        private readonly IObservableOperationRepository _observableOperationRepository;
        private readonly OperationsConfirmationsSettings _confirmationsSettings;
        private readonly ILog _log;
        private readonly IOperationMetaRepository _operationMetaRepository;
        private readonly IOperationEventRepository _operationEventRepository;
        private readonly IWalletBalanceService _walletBalanceService;
        
        public UpdateObservableOperations(IUnconfirmedTransactionRepository unconfirmedTransactionRepository, 
            IBlockChainProvider blockChainProvider,
            IObservableOperationRepository observableOperationRepository, 
            OperationsConfirmationsSettings confirmationsSettings,
            ILog log,
            IOperationMetaRepository operationMetaRepository, 
            IOperationEventRepository operationEventRepository,
            IWalletBalanceService walletBalanceService)
        {
            _unconfirmedTransactionRepository = unconfirmedTransactionRepository;
            _blockChainProvider = blockChainProvider;
            _observableOperationRepository = observableOperationRepository;
            _confirmationsSettings = confirmationsSettings;
            _log = log;
            _operationMetaRepository = operationMetaRepository;
            _operationEventRepository = operationEventRepository;
            _walletBalanceService = walletBalanceService;
        }

        [TimerTrigger("00:02:00")]
        public async Task DetectUnconfirmedTransactions()
        {
            var unconfirmedTxs = await _unconfirmedTransactionRepository.GetAll();

            foreach (var unconfirmedTransaction in unconfirmedTxs)
            {
                var operationMeta =await _operationMetaRepository.Get(unconfirmedTransaction.OperationId);
                if (operationMeta == null)
                {
                    await _log.WriteWarningAsync(nameof(UpdateObservableOperations), nameof(DetectUnconfirmedTransactions),
                        unconfirmedTransaction.ToJson(), "OperationMeta not found");
                    continue;
                }

                var confirmationCount = await _blockChainProvider.GetTxConfirmationCount(unconfirmedTransaction.TxHash);

                var isCompleted = confirmationCount >= _confirmationsSettings.MinConfirmationsToDetectOperation;
;
                if (isCompleted)
                {
                    //Force update balances
                    var fromAddressUpdatedBalance = await _walletBalanceService.UpdateBalance(operationMeta.FromAddress);
                    var toAddressUpdatedBalance = await _walletBalanceService.UpdateBalance(operationMeta.ToAddress);
                    

                    var operationCompletedLoggingContext = new
                    {
                        unconfirmedTransaction.OperationId,
                        unconfirmedTransaction.TxHash,
                        fromAddressUpdatedBalance,
                        toAddressUpdatedBalance
                    };

                    await _operationEventRepository.InsertIfNotExist(OperationEvent.Create(unconfirmedTransaction.OperationId,
                        OperationEventType.DetectedOnBlockChain, operationCompletedLoggingContext));

                    await _log.WriteInfoAsync(nameof(UpdateBalanceFunctions), nameof(DetectUnconfirmedTransactions),
                        operationCompletedLoggingContext.ToJson(),
                        "Operation completed");


                    await _unconfirmedTransactionRepository.DeleteIfExist(unconfirmedTransaction.OperationId);
                }

                var status = isCompleted
                    ? BroadcastStatus.Completed
                    : BroadcastStatus.InProgress;

                await _observableOperationRepository.InsertOrReplace(ObervableOperation.Create(operationMeta, status,
                    unconfirmedTransaction.TxHash));
            }
        }
    }
}
