﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashIn
{

    public enum CashInEventType
    {
       MoneyTransferredToHotWallet,
       MoneyTransferredToHoRetry
    }

    public interface ICashInEvent
    {
        CashInEventType Type { get; }

        DateTime DateTime { get; }

        Guid OperationId { get; }
    }

    public class CashInEvent : ICashInEvent
    {
        public CashInEventType Type { get; set; }
        public DateTime DateTime { get; set; }
        public Guid OperationId { get; set; }

        public static CashInEvent Create(Guid operationId, CashInEventType type, DateTime? dateTime = null)
        {
            return new CashInEvent
            {
                DateTime = dateTime??DateTime.UtcNow,
                OperationId = operationId,
                Type = type
            };
        }
    }

    public interface ICashInEventRepository
    {
        Task InsertEvent(ICashInEvent cashInEvent);
        Task<bool> Exist(Guid operationId, CashInEventType type);
    }
}
