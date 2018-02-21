﻿using System;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.Vertcoin.API.Core.Operation
{
    public interface IOperationService
    {
        Task<Transaction> GetOrBuildTransferTransaction(Guid operationId, 
            BitcoinAddress fromAddress,
            BitcoinAddress toAddress,
            string assetId,
            Money amountToSend, 
            bool includeFee);
    }
}
