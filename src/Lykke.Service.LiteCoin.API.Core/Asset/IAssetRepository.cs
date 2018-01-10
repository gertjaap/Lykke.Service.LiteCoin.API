using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Asset
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
        Task<IEnumerable<IAsset>> GetAll();
        Task<IAsset> GetById(string assetId);
    }
}
