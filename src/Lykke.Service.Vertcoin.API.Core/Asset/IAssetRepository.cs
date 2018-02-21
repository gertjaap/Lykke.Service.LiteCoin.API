using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Vertcoin.API.Core.Pagination;

namespace Lykke.Service.Vertcoin.API.Core.Asset
{
    public interface IAsset
    {
        string AssetId { get; }
        string Address { get; }
        string Name { get; }
        int Accuracy { get; }
    }

    public interface IAssetRepository
    {
        Task<IPaginationResult<IAsset>> GetPaged(int take, string continuation);
        Task<IAsset> GetById(string assetId);
    }
}
