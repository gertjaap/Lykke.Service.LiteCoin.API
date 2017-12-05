using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Fee
{
    public interface IFeeFacade
    {
        Task<Money> CalcFeeForTransaction(Transaction tx);
        Task<Money> CalcFeeForTransaction(TransactionBuilder builder);
    }
}
