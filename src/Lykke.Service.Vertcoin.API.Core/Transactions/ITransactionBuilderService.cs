﻿using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.Vertcoin.API.Core.Transactions
{
    public interface IBuildedTransaction
    {
        Transaction TransactionData { get; }
        Money Fee { get; }
        Money Amount { get; }
    }
    public interface ITransactionBuilderService
    {
        Task<IBuildedTransaction> GetTransferTransaction(BitcoinAddress source, BitcoinAddress destination, Money amount, bool includeFee);

    }
}
