using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Transactions
{
    public interface ITransactionBuilderService
    {
        Task<Transaction> GetTransferTransaction(BitcoinAddress source, BitcoinAddress destination, decimal amount, bool sentDust = false);
        Task<Transaction> GetSendMoneyToHotWalletTransaction(BitcoinAddress fromAddress, BitcoinAddress destination, string fromTxHash);

    }
}
