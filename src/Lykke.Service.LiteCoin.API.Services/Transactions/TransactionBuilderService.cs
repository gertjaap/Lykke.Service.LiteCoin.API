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
    public class BuildedTransaction: IBuildedTransaction
    {
        public Transaction TransactionData { get; set; }
        public Money Fee { get; set; }
        public Money Amount { get; set; }

        public static BuildedTransaction Create(Transaction transaction, Money fee, Money amount)
        {
            return new BuildedTransaction
            {
                Amount = amount,
                Fee = fee,
                TransactionData = transaction
            };
        }
    }

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

        public async Task<IBuildedTransaction> GetTransferTransaction(BitcoinAddress source,
            BitcoinAddress destination, Money amount, bool includeFee)
        {
            var builder = new TransactionBuilder();

            return await TransferOneDirection(builder, source, amount.Satoshi, destination, includeFee);
            
        }

        private async Task<IBuildedTransaction> TransferOneDirection(TransactionBuilder builder, 
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

            return await SendWithChange(builder,  coins, to, new Money(amount),
                from, includeFee);
        }

        public async Task<IBuildedTransaction> SendWithChange(TransactionBuilder builder, 
            List<Coin> coins, IDestination destination, Money amount, IDestination changeDestination, bool includeFee)
        {
            if (amount.Satoshi <= 0)
                throw new BusinessException("Amount can't be less or equal to zero", ErrorCode.BadInputParameter);


            var orderedCoins = coins.OrderBy(o => o.Amount).ToList(); //use settled in blockchain outputs first
            var sendAmount = Money.Zero;
            var cnt = GetCoinsCount(amount, orderedCoins, ref sendAmount);

            builder.AddCoins(orderedCoins.Take(cnt))
                .Send(destination, amount)
                .SetChange(changeDestination);
            
            var fee = await _feeService.CalcFeeForTransaction(builder);

            if (!includeFee && sendAmount < amount + fee)
            {
                var orderedFeeCoins = orderedCoins.Skip(cnt)
                    .OrderBy(o => o.Amount)
                    .ToList();

                var feeCoinsCnt = GetCoinsCount(fee, orderedFeeCoins, ref sendAmount);
                builder.AddCoins(orderedFeeCoins.Take(feeCoinsCnt));
            }

            if (fee >= amount)
            {
                throw new BusinessException(
                    $"The sum of total applicable outputs is less than the required fee: {fee} satoshis.",
                    ErrorCode.BalanceIsLessThanFee);
            }
            
            if (includeFee)
            {
                builder.SubtractFees();
                amount = amount - fee;
            }

            if (sendAmount < amount + fee)
            {
                throw new BusinessException(
                    $"The sum of total applicable outputs is less than the required: {amount.Satoshi} satoshis.",
                    ErrorCode.NotEnoughFundsAvailable);
            }
            
            builder.SendFees(fee);

            var tx = builder.BuildTransaction(false);

            return BuildedTransaction.Create(tx, fee, amount);
        }

        private static int GetCoinsCount(Money amount, List<Coin> orderedCoins, ref Money sendAmount)
        {
            var cnt = 0;
            while (sendAmount < amount && cnt < orderedCoins.Count)
            {
                sendAmount += orderedCoins[cnt].TxOut.Value;
                cnt++;
            }

            return cnt;
        }
    }

}
