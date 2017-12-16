using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.CashOut;

namespace Lykke.Job.LiteCoin.OperationsDetector.Functions
{
    public class DetectCashOutFunctions
    {
        private readonly ICashOutsOperationDetectorFacade _detector;

        public DetectCashOutFunctions(ICashOutsOperationDetectorFacade detector)
        {
            _detector = detector;
        }

        [TimerTrigger("00:10:00")]
        public async Task HandleCashOutCompleted()
        {
            await _detector.DetectCashOutOps();
        }
    }
}
