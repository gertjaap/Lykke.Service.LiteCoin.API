using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Services.Operations;

namespace Lykke.Job.LiteCoin.Functions
{
    public class RemoveOldCashInsFunctions
    {
        private readonly OperationsExpirationSettings _settings;
        private readonly ICashInOperationRepository _cashInOperationRepository;
            

        public RemoveOldCashInsFunctions(OperationsExpirationSettings settings, ICashInOperationRepository cashInOperationRepository)
        {
            _settings = settings;
            _cashInOperationRepository = cashInOperationRepository;
        }

        [TimerTrigger("1.00:00:00")]
        public  Task Clean()
        {
            var bound = DateTime.UtcNow.AddDays(-_settings.CashInExpirationDays);

            return _cashInOperationRepository.DeleteOldOperations(bound);
        }
    }
}
