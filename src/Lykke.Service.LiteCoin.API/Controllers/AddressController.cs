using System.Net;
using Lykke.Service.BlockchainApi.Contract.Responses;
using Lykke.Service.LiteCoin.API.Core.Address;
using Lykke.Service.LiteCoin.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class AddressController:Controller
    {
        private readonly IAddressValidator _addressValidator;

        public AddressController(IAddressValidator addressValidator)
        {
            _addressValidator = addressValidator;
        }
        [SwaggerOperation(nameof(Validate))]
        [ProducesResponseType(typeof(AddressValidationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [HttpGet("api/addresses/{address}/is-valid")]
        public AddressValidationResponse Validate(string address)
        {
            return new AddressValidationResponse
            {
                IsValid = _addressValidator.IsValid(address)
            };
        }
    }
}
