using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.ObservableOperation
{
    public interface IObservableOperationService
    {
        Task<IEnumerable<IObservableOperation>> GetInProgressOperations(int skip, int take);
        Task<IEnumerable<IObservableOperation>> GetCompletedOperations(int skip, int take);
        Task<IEnumerable<IObservableOperation>> GetFailedOperations(int skip, int take);
        Task DeleteOperations(IEnumerable<Guid> opIds);
    }
}
