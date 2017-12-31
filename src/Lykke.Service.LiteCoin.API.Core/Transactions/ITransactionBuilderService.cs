using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Transactions
{
    public interface ITransactionBuilderService
    {
        Task<Transaction> GetTransferTransaction(BitcoinAddress source, BitcoinAddress destination, long amount, bool sentDust = false);
        Task<Transaction> GetSendAllTransaction(BitcoinAddress fromAddress, BitcoinAddress destination, string fromTxHash);

    }
}
