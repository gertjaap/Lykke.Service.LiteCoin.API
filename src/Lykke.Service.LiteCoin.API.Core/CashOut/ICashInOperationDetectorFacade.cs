using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashOut
{
    public interface ICashOutsOperationDetectorFacade
    {
        Task DetectCashOutOps();
    }
}
