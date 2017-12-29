using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract.Responses;
using Lykke.Service.LiteCoin.API.Core.Asset;
using Lykke.Service.LiteCoin.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class AssetsController: Controller
    {
        private readonly IAssetRepository _assetRepository;

        public AssetsController(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        [SwaggerOperation(nameof(GetAll))]
        [ProducesResponseType(typeof(AssetsListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [HttpGet("api/assets")]
        public async Task<AssetsListResponse> GetAll()
        {
            var assets = await _assetRepository.GetAll();

            return new AssetsListResponse
            {
                Assets = assets.Select(p => new AssetResponse
                {
                    Address = p.Address,
                    AssetId = p.AssetId,
                    Accuracy = p.Accuracy,
                    Name = p.Name
                }).ToList().AsReadOnly()
            };
        }

        [SwaggerOperation(nameof(GetById))]
        [ProducesResponseType(typeof(AssetResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(AssetResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [HttpGet("api/assets/{assetId}")]
        public async Task<IActionResult> GetById(string assetId)
        {
            var asset = await _assetRepository.GetById(assetId);
            if (asset == null)
            {
                return NotFound();
            }

            return Ok(new AssetResponse
            {
                Address = asset.Address,
                AssetId = asset.AssetId,
                Accuracy = asset.Accuracy,
                Name = asset.Name
            });
        }
    }
}
