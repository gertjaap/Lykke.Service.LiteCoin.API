using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Fee;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Transactions
{
    public class TransactionBuilderService : ITransactionBuilderService
    {
        private readonly ITransactionOutputsService _transactionOutputsService;
        private readonly IFeeService _feeService;

        public TransactionBuilderService(ITransactionOutputsService transactionOutputsService,
            IFeeService feeService)
        {
            _transactionOutputsService = transactionOutputsService;
            _feeService = feeService;
        }

        public async Task<Transaction> GetTransferTransaction(BitcoinAddress source,
            BitcoinAddress destination, Money amount, bool includeFee)
        {
            var builder = new TransactionBuilder();

            await TransferOneDirection(builder, source, amount.Satoshi, destination, includeFee);


            var buildedTransaction = builder.BuildTransaction(false);

            return buildedTransaction;
        }

        private async Task TransferOneDirection(TransactionBuilder builder, 
            BitcoinAddress @from, long amount, BitcoinAddress to, bool includeFee)
        {
            var fromStr = from.ToString();
            var coins = (await _transactionOutputsService.GetUnspentOutputs(fromStr)).ToList();
            var balance = coins.Select(o => o.Amount).DefaultIfEmpty()
                .Sum(o =>  o?.Satoshi ?? 0);
            if (balance > amount &&
                balance - amount < new TxOut(Money.Zero, from)
                    .GetDustThreshold(builder.StandardTransactionPolicy.MinRelayTxFee).Satoshi)
                amount = balance;
            await SendWithChange(builder,  coins, to, new Money(amount),
                from, includeFee);
        }

        public async Task<long> SendWithChange(TransactionBuilder builder, 
            List<CoinWithSettlementInfo> coins, IDestination destination, Money amount, IDestination changeDestination, bool includeFee)
        {
            if (amount.Satoshi <= 0)
                throw new BusinessException("Amount can't be less or equal to zero", ErrorCode.BadInputParameter);


            var orderedCoins = coins.OrderByDescending(p => p.IsSettled).ThenBy(o => o.Amount).ToList(); //use settled in blockchain outputs first
            var sendAmount = Money.Zero;
            var cnt = GetCoinsCount(amount, orderedCoins, ref sendAmount);

            builder.AddCoins(orderedCoins.Take(cnt));

            Money fee;

            if (includeFee)
            {
                fee = await _feeService.GetMinFee();

                if (fee >= amount)
                {
                    throw new BusinessException(
                        $"The sum of total applicable outputs is less than the required fee: {fee} satoshis.",
                        ErrorCode.BalanceIsLessThanFee);
                }
            }
            else
            {
                fee = await _feeService.CalcFeeForTransaction(builder);

                if (fee >= amount)
                {
                    throw new BusinessException(
                        $"The sum of total applicable outputs is less than the required fee: {fee} satoshis.",
                        ErrorCode.BalanceIsLessThanFee);
                }

                amount = amount - fee;
            }

            if (sendAmount < amount + fee)
            {
                var orderedFeeCoins = orderedCoins.Skip(cnt)
                    .OrderBy(o => o.Amount)
                    .ToList();

                var feeCoinsCnt = 0;

                while (sendAmount < amount + fee && feeCoinsCnt < orderedFeeCoins.Count)
                {
                    sendAmount += orderedFeeCoins[cnt].TxOut.Value;
                    feeCoinsCnt++;
                }

                builder.AddCoins(orderedFeeCoins.Take(feeCoinsCnt));
            }

            builder.Send(destination, amount);
            builder.SetChange(changeDestination);

            builder.SendFees(fee);
            builder.BuildTransaction(false);
            

            return amount;
        }

        private static int GetCoinsCount(Money amount, List<CoinWithSettlementInfo> orderedCoins, ref Money sendAmount)
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
