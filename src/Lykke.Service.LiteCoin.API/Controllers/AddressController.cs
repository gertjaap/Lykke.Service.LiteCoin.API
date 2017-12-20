using Lykke.Service.LiteCoin.API.Core.Address;
using Lykke.Service.LiteCoin.API.Models.Address;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class AddressController:Controller
    {
        private readonly IAddressValidator _addressValidator;

        public AddressController(IAddressValidator addressValidator)
        {
            _addressValidator = addressValidator;
        }

        [HttpPost("api/address/validator")]
        public AddressValidatorResponce Validate([FromBody]AddressValidatorRequest request)
        {
            return new AddressValidatorResponce
            {
                IsValid = _addressValidator.IsValid(request.Address)
            };
        }
    }
}
