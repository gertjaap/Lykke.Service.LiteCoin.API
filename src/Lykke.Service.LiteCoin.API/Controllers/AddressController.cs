using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Models.Address;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class AddressController:Controller
    {

        [HttpPost("api/address/validator")]
        public AddressValidatorResponce Validate([FromBody]AddressValidatorRequest request)
        {
            try
            {
                BitcoinAddress.Create(request.Address);
                return new AddressValidatorResponce
                {
                    IsValid = true
                };
            }
            catch(Exception)
            {
                return new AddressValidatorResponce
                {
                    IsValid = false
                };
            }
        }
    }
}
