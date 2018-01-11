using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.ObservableOperation
{
    public interface IObservableOperationService
    {
        Task<IEnumerable<IObservableOperation>> GetInProgressOperations();
        Task<IEnumerable<IObservableOperation>> GetCompletedOperations();
        Task<IEnumerable<IObservableOperation>> GetFailedOperations();
        Task DeleteOperations(IEnumerable<Guid> opIds);
    }
}
