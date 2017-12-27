using System;

namespace Lykke.Service.LiteCoin.API.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public ErrorCode Code { get; private set; }
        public string Text { get; private set; }

        public BusinessException(string text, ErrorCode code)
            : base(text)
        {
            Code = code;
            Text = text;
        }
    }

    public enum ErrorCode
    {
        Exception = 0,
        CantFindAddressToSignTx = 1,
        TransactionConcurrentInputsProblem = 2,
        BadInputParameter = 3,
        NotEnoughFundsAvailable = 4,
        CashOutOperationNotFound = 5,
        CashInOperationNotFound = 6,
        WalletNotFound = 7,
        BroadcastError = 8,
        SignError = 9,
        BalanceIsLessThanFee = 10

    }
}
