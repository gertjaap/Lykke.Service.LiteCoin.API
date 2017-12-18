using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Transactions
{
    public interface IBuildedTx
    {
        Transaction Transaction { get; }
    }

    public interface ITransactionBuilderService
    {
        Task<IBuildedTx> GetTransferTransaction(BitcoinAddress source, BitcoinAddress destination, decimal amount, bool sentDust = false);

    }
}
