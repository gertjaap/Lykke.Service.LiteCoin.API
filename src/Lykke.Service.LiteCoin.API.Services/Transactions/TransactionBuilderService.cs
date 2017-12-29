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
    public class TransactionBuilderService : ITransactionBuilderService
    {
        private readonly ITransactionOutputsService _transactionOutputsService;
        private readonly ILog _log;
        private readonly IFeeService _feeService;

        public TransactionBuilderService(ITransactionOutputsService transactionOutputsService,
            ILog log,
            IFeeService feeService)
        {
            _transactionOutputsService = transactionOutputsService;
            _log = log;
            _feeService = feeService;
        }

        public async Task<Transaction> GetTransferTransaction(BitcoinAddress source,
            BitcoinAddress destination, long amount, bool sentDust = false)
        {
            var builder = new TransactionBuilder();

            await TransferOneDirection(builder, source, (long)amount, destination, sentDust);


            var buildedTransaction = builder.BuildTransaction(false);

            return buildedTransaction;
        }

        public async Task<Transaction> GetSendMoneyToHotWalletTransaction(BitcoinAddress fromAddress, BitcoinAddress destination, string fromTxHash)
        {
            var builder = new TransactionBuilder();
            var outputs = (await _transactionOutputsService.GetUnspentOutputs(fromAddress.ToString())).Where(p => p.Outpoint.Hash.ToString() == fromTxHash).ToArray();
            var amount = outputs.Sum(o => o.Amount);
            var fee = await _feeService.GetMinFee();

            if (fee.Satoshi > amount)
            {
                throw new BusinessException(
                    $"The sum of total applicable outputs is less than the required fee: {fee.Satoshi} satoshis.",
                    ErrorCode.BalanceIsLessThanFee);
            }

            builder.AddCoins(outputs.Cast<Coin>());
            builder.Send(destination, amount - fee);
            builder.SendFees(fee);

            return builder.BuildTransaction(false);
        }

        private async Task TransferOneDirection(TransactionBuilder builder, 
            BitcoinAddress @from, long amount, BitcoinAddress to, bool addDust = true, bool sendDust = false)
        {
            var fromStr = from.ToString();
            var coins = (await _transactionOutputsService.GetUnspentOutputs(fromStr)).ToList();
            var balance = coins.Select(o => o.Amount).DefaultIfEmpty()
                .Sum(o =>  o?.Satoshi ?? 0);
            if (sendDust && balance > (long) amount &&
                balance - (long)amount < new TxOut(Money.Zero, from)
                    .GetDustThreshold(builder.StandardTransactionPolicy.MinRelayTxFee).Satoshi)
                amount = balance;
            await SendWithChange(builder,  coins, to, new Money(amount),
                from, addDust);
        }

        public async Task<long> SendWithChange(TransactionBuilder builder, 
            List<CoinWithSettlementInfo> coins, IDestination destination, Money amount, IDestination changeDestination,
            bool addDust = true)
        {
            if (amount.Satoshi <= 0)
                throw new BusinessException("Amount can't be less or equal to zero", ErrorCode.BadInputParameter);


            var orderedCoins = coins.OrderByDescending(p => p.IsSettled).ThenBy(o => o.Amount).ToList(); //use settled in blockchain outputs first
            var sendAmount = Money.Zero;
            var cnt = NewMethod(amount, orderedCoins, ref sendAmount);

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

                builder.AddCoins(orderedFeeCoins.Take(feeCoinsCnt));
            }


            builder.Send(destination, amount);
            builder.SetChange(changeDestination);


            builder.SendFees(precalculatedFee);
            builder.BuildTransaction(false);



            return amount;
        }

        private static int NewMethod(Money amount, List<CoinWithSettlementInfo> orderedCoins, ref Money sendAmount)
        {
            var cnt = 0;
            while (sendAmount < amount && cnt < orderedCoins.Count)
            {
                sendAmount += orderedCoins[cnt].TxOut.Value;
                cnt++;
            }

            if (sendAmount < amount)
            {
                throw new BusinessException(
                    $"The sum of total applicable outputs is less than the required: {amount.Satoshi} satoshis.",
                    ErrorCode.NotEnoughFundsAvailable);
            }

            return cnt;
        }
    }

}
