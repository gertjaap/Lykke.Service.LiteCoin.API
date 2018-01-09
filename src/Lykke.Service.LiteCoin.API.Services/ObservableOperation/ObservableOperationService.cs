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

        public Task<IEnumerable<IObservableOperation>> GetInProgressOperations(int skip, int take)
        {
            return _observableOperationRepository.Get(BroadcastStatus.InProgress, skip, take);
        }

        public Task<IEnumerable<IObservableOperation>> GetCompletedOperations(int skip, int take)
        {
            return _observableOperationRepository.Get(BroadcastStatus.Completed, skip, take);
        }

        public Task<IEnumerable<IObservableOperation>> GetFailedOperations(int skip, int take)
        {
            return _observableOperationRepository.Get(BroadcastStatus.Failed, skip, take);
        }

        public async Task DeleteOperations(IEnumerable<Guid> opIds)
        {
            var enumerable = opIds as Guid[] ?? opIds.ToArray();

            await _observableOperationRepository.DeleteIfExist(enumerable.ToArray());
            await _unconfirmedTransactionRepository.DeleteIfExist(enumerable.ToArray());
        }
    }
}
