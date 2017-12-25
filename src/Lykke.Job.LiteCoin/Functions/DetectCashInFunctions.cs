using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Services.Operations.CashIn;

namespace Lykke.Job.LiteCoin.Functions
{
    public class DetectCashInFunctions
    {
        private readonly ICashInOperationDetectorFacade _cashInOperationDetectorFacade;

        public DetectCashInFunctions(ICashInOperationDetectorFacade cashInOperationDetectorFacade)
        {
            _cashInOperationDetectorFacade = cashInOperationDetectorFacade;
        }

        [TimerTrigger("00:10:00")]
        public async Task Detect()
        {
            await _cashInOperationDetectorFacade.DetectCashInOps();
        }
    }
}
