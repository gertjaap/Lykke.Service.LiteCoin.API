using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;

namespace Lykke.Job.LiteCoin.OperationsDetector.AzureQueueHandlers
{
    public class DetectCashOutFunctions
    {
        [TimerTrigger("00:10:00")]
        public async Task DetectCashIn()
        {

            
        }
    }
}
