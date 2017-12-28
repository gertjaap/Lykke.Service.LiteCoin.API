﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashOut
{
    public interface ICashoutTransaction
    {
        string TxHash { get; }
        Guid OperationId { get; }
        DateTime InsertedAt { get; }
    }

    public class CashOutTransaction: ICashoutTransaction
    {
        public string TxHash { get; set; }
        public Guid OperationId { get; set; }
        public DateTime InsertedAt { get; set; }


        public static CashOutTransaction Create(string txHash, Guid operationId, DateTime? insertedAt = null)
        {
            return new CashOutTransaction
            {
                OperationId = operationId,
                TxHash = txHash,
                InsertedAt = insertedAt ?? DateTime.UtcNow
            };
        }
    }
    public interface IPendingCashoutTransactionRepository
    {
        Task<IEnumerable<ICashoutTransaction>> GetAll();
        Task InsertOrReplace(ICashoutTransaction tx);
        Task Remove(string txHash);
    }
}
