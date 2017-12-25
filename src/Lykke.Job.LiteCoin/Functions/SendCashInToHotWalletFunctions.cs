using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.Fee;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Core.Sign;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using NBitcoin;

namespace Lykke.Job.LiteCoin.Functions
{
    public class SendCashInToHotWalletFunctions
    {
        private readonly IWalletService _walletService;
        private readonly ITransactionOutputsService _outputsService;
        private readonly ICashInOperationRepository _cashInOperationRepository;
        private readonly ILog _log;
        private readonly IFeeService _feeService;
        private readonly ISignService _signService;
        private readonly IBlockChainProvider _blockChainProvider;
        private readonly ITransactionBlobStorage _transactionBlobStorage;

        public SendCashInToHotWalletFunctions(IWalletService walletService,
            ITransactionOutputsService outputsService, 
            ICashInOperationRepository cashInOperationRepository,
            ILog log,
            IFeeService feeService, 
            ISignService signService,
            IBlockChainProvider blockChainProvider, 
            ITransactionBlobStorage transactionBlobStorage)
        {
            _walletService = walletService;
            _outputsService = outputsService;
            _cashInOperationRepository = cashInOperationRepository;
            _log = log;
            _feeService = feeService;
            _signService = signService;
            _blockChainProvider = blockChainProvider;
            _transactionBlobStorage = transactionBlobStorage;
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

            var hotWallet = (await _walletService.GetHotWallets()).First();
            
            var outputs = (await _outputsService.GetOnlyBlockChainUnspentOutputs(operation.DestinationAddress)).ToList();
            var balance = outputs.Sum(o => o.Amount);

            if (outputs.Any())
            {
                var builder = new TransactionBuilder();
                builder.AddCoins(outputs).Send(hotWallet.Address, balance);

                var fee = await _feeService.CalcFeeForTransaction(builder);

                if (fee > balance)
                {
                    await _log.WriteWarningAsync(nameof(SendCashInToHotWalletFunctions), nameof(Send), context.ToJson(),
                            $"Calculated fee is more than balance ({fee.Satoshi}>{balance})");
                    return;
                }

                builder.SendFees(fee);

                var unsignedTx = builder.BuildTransaction(true);

                await _transactionBlobStorage.AddOrReplaceTransaction(operation.OperationId, 
                    TransactionBlobType.Initial,
                    unsignedTx.ToHex());

                var signedTx = await _signService.SignTransaction(context.OperationId, unsignedTx, clientWallet.Address);

                await _transactionBlobStorage.AddOrReplaceTransaction(operation.OperationId,
                    TransactionBlobType.Signed,
                    signedTx.ToHex());

                await _blockChainProvider.BroadCastTransaction(signedTx);
            }
            else
            {
                await _log.WriteWarningAsync(nameof(SendCashInToHotWalletFunctions), nameof(Send), context.ToJson(),
                    "Not found CashIn outputs");
            }
        }
    }
}
