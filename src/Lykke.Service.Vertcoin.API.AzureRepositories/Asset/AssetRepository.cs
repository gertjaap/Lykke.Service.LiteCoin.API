using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.AzureStorage.Tables.Paging;
using Lykke.Service.Vertcoin.API.Core.Asset;
using Lykke.Service.Vertcoin.API.Core.Constants;
using Lykke.Service.Vertcoin.API.Core.Pagination;

namespace Lykke.Service.Vertcoin.API.AzureRepositories.Asset
{
    public class Asset : IAsset
    {
        public string AssetId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public int Accuracy { get; set; }
    }

    public class AssetRepository:IAssetRepository
    {
        private readonly IEnumerable<IAsset> _mockList = new List<IAsset>
        {

            new Asset
            {
                Address = Constants.Assets.Vertcoin.Address,
                AssetId = Constants.Assets.Vertcoin.AssetId,
                Accuracy = Constants.Assets.Vertcoin.Accuracy,
                Name = Constants.Assets.Vertcoin.Name
            }
        };

        public Task<IPaginationResult<IAsset>> GetPaged(int take, string continuation)
        {
            return Task.FromResult(PaginationResult<IAsset>.Create(_mockList, null));
        }

        public Task<IAsset> GetById(string assetId)
        {
            return Task.FromResult(_mockList.SingleOrDefault(p=>p.AssetId == assetId));
        }
    }
}
