using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Asset;
using Lykke.Service.LiteCoin.API.Core.Constants;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Asset
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
                Address = "",
                AssetId = Constants.AssetsContants.LiteCoin,
                Accuracy = 8,
                Name = "LiteCoin"
            }
        };

        public Task<IEnumerable<IAsset>> GetAll()
        {
            return Task.FromResult(_mockList);
            throw new NotImplementedException();
        }

        public Task<IAsset> GetById(string assetId)
        {
            return Task.FromResult(_mockList.SingleOrDefault(p=>p.AssetId == assetId));
        }
    }
}
