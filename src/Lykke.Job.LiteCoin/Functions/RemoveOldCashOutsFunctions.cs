using System;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Services.Operations;

namespace Lykke.Job.LiteCoin.Functions
{
    public class RemoveOldCashOutsFunctions
    {
        private readonly OperationsExpirationSettings _settings;
        private readonly ICashOutOperationRepository _cashOutOperationRepository;

        public RemoveOldCashOutsFunctions(OperationsExpirationSettings settings, ICashOutOperationRepository cashOutOperationRepository)
        {
            _settings = settings;
            _cashOutOperationRepository = cashOutOperationRepository;
        }


        [TimerTrigger("1.00:00:00")]
        public  Task Clean()
        {
            var bound = DateTime.UtcNow.AddDays(-_settings.CashOutExpirationDays);

            return _cashOutOperationRepository.DeleteOldOperations(bound);
        }
    }
}
