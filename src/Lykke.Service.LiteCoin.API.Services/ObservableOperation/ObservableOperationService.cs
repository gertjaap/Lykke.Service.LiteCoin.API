using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.ObservableOperation;
using Lykke.Service.LiteCoin.API.Core.Transactions;

namespace Lykke.Service.LiteCoin.API.Services.ObservableOperation
{
    public class ObservableOperationService: IObservableOperationService
    {
        private readonly IObservableOperationRepository _observableOperationRepository;
        private readonly IUnconfirmedTransactionRepository _unconfirmedTransactionRepository;

        public ObservableOperationService(IObservableOperationRepository observableOperationRepository, IUnconfirmedTransactionRepository unconfirmedTransactionRepository)
        {
            _observableOperationRepository = observableOperationRepository;
            _unconfirmedTransactionRepository = unconfirmedTransactionRepository;
        }

        public Task<IEnumerable<IObservableOperation>> GetInProgressOperations()
        {
            return _observableOperationRepository.Get(BroadcastStatus.InProgress);
        }

        public Task<IEnumerable<IObservableOperation>> GetCompletedOperations()
        {
            return _observableOperationRepository.Get(BroadcastStatus.Completed);
        }

        public Task<IEnumerable<IObservableOperation>> GetFailedOperations()
        {
            return _observableOperationRepository.Get(BroadcastStatus.Failed);
        }

        public async Task DeleteOperations(IEnumerable<Guid> opIds)
        {
            var enumerable = opIds as Guid[] ?? opIds.ToArray();

            await _observableOperationRepository.DeleteIfExist(enumerable.ToArray());
            await _unconfirmedTransactionRepository.DeleteIfExist(enumerable.ToArray());
        }
    }
}
