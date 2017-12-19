using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Fee;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using Lykke.Service.LiteCoin.API.Services.Helpers;
using NBitcoin;
using NBitcoin.Policy;

namespace Lykke.Service.LiteCoin.API.Services.Transactions
{
    public class BuildedTx : IBuildedTx
    {
        public Transaction Transaction { get; set; }
    }

    public class TransactionBuilderService : ITransactionBuilderService
    {
        private readonly ITransactionOutputsService _transactionOutputsService;
        private readonly TransactionBuildContextFactory _transactionBuildContextFactory;
        private readonly ILog _log;
        private readonly ISpentOutputService _spentOutputService;
        private readonly IBroadcastedOutputsService _broadcastedOutputsService;
        private readonly IFeeService _feeService;

        public TransactionBuilderService(ITransactionOutputsService transactionOutputsService,
            TransactionBuildContextFactory transactionBuildContextFactory,
            ILog log, ISpentOutputService spentOutputService,
            IBroadcastedOutputsService broadcastedOutputsService,
            IFeeService feeService)
        {
            _transactionOutputsService = transactionOutputsService;
            _transactionBuildContextFactory = transactionBuildContextFactory;
            _log = log;
            _spentOutputService = spentOutputService;
            _broadcastedOutputsService = broadcastedOutputsService;
            _feeService = feeService;
        }

        public async Task<IBuildedTx> GetTransferTransaction(BitcoinAddress source,
            BitcoinAddress destination, decimal amount, bool sentDust = false)
        {
            return await Retry.Try(async () =>
                {
                    var context = _transactionBuildContextFactory.Create();

                    return await context.Build(async () =>
                    {
                        var builder = new TransactionBuilder();

                        await TransferOneDirection(builder, context, source, amount, destination, sentDust);


                        var buildedTransaction = builder.BuildTransaction(true);


                        await _spentOutputService.SaveSpentOutputs(buildedTransaction);

                        await _broadcastedOutputsService.SaveNewOutputs(buildedTransaction);

                        return new BuildedTx
                        {
                            Transaction = buildedTransaction
                        };

                    });
                }, exception => (exception as BackendException)?.Code == ErrorCode.TransactionConcurrentInputsProblem,
                3,
                _log);
        }

        private async Task TransferOneDirection(TransactionBuilder builder, TransactionBuildContext context,
            BitcoinAddress @from, decimal amount, BitcoinAddress to, bool addDust = true, bool sendDust = false)
        {
            var fromStr = from.ToString();
            var coins = (await _transactionOutputsService.GetUnspentOutputs(fromStr)).ToList();
            var balance = coins.Select(o => o.Amount).DefaultIfEmpty()
                .Sum(o => o?.ToDecimal(MoneyUnit.BTC) ?? 0);
            if (sendDust && balance > amount &&
                balance - amount < new TxOut(Money.Zero, from)
                    .GetDustThreshold(builder.StandardTransactionPolicy.MinRelayTxFee).ToDecimal(MoneyUnit.BTC))
                amount = balance;
            await SendWithChange(builder, context, coins, to, new Money(amount, MoneyUnit.BTC),
                from, addDust);
        }

        public async Task<decimal> SendWithChange(TransactionBuilder builder, TransactionBuildContext context,
            List<Coin> coins, IDestination destination, Money amount, IDestination changeDestination,
            bool addDust = true)
        {
            if (amount.Satoshi <= 0)
                throw new BackendException("Amount can't be less or equal to zero", ErrorCode.BadInputParameter);

            void ThrowError()
            {
                throw new BackendException(
                    $"The sum of total applicable outputs is less than the required: {amount.Satoshi} satoshis.",
                    ErrorCode.NotEnoughFundsAvailable);
            }
            
            var orderedCoins = coins.OrderBy(o => o.Amount).ToList();
            var sendAmount = Money.Zero;
            var cnt = 0;
            while (sendAmount < amount && cnt < orderedCoins.Count)
            {
                sendAmount += orderedCoins[cnt].TxOut.Value;
                cnt++;
            }
            if (sendAmount < amount)
                ThrowError();

            context.AddCoins(orderedCoins.Take(cnt));
            builder.AddCoins(orderedCoins.Take(cnt));

            var precalculatedFee = await _feeService.CalcFeeForTransaction(builder);

            if (sendAmount < amount + precalculatedFee)
            {
                var orderedFeeCoins = orderedCoins.Skip(cnt)
                    .OrderBy(o => o.Amount)
                    .ToList();

                var feeCoinsCnt = 0;

                while (sendAmount < amount + precalculatedFee && feeCoinsCnt < orderedFeeCoins.Count)
                {
                    sendAmount += orderedFeeCoins[cnt].TxOut.Value;
                    feeCoinsCnt++;
                }

                context.AddCoins(orderedFeeCoins.Take(feeCoinsCnt));
                builder.AddCoins(orderedFeeCoins.Take(feeCoinsCnt));
            }

            builder.SendFees(precalculatedFee);

            var sent = await Send(builder, destination, amount, addDust);

            if (sendAmount - amount > 0)
                await Send(builder, changeDestination, sendAmount - amount, addDust);
            return sent;
        }

        private Task<decimal> Send(TransactionBuilder builder, IDestination destination, Money amount, bool addDust)
        {
            var newAmount = Money.Max(GetDust(destination, addDust), amount);
            builder.Send(destination, newAmount);

            return Task.FromResult(newAmount.ToDecimal(MoneyUnit.BTC));
        }

        private Money GetDust(IDestination destination, bool addDust = true)
        {
            return addDust
                ? new TxOut(Money.Zero, destination.ScriptPubKey).GetDustThreshold(new StandardTransactionPolicy()
                    .MinRelayTxFee)
                : Money.Zero;
        }
    }

}
