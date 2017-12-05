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
        private readonly Network _network;

        public AddressController(Network network)
        {
            _network = network;
        }

        [HttpGet("api/address/validator")]
        public AddressValidatorResponce Validate(AddressValidatorRequest request)
        {
            try
            {

                BitcoinAddress.Create(request.Address, _network);
                return new AddressValidatorResponce
                {
                    IsValid = true
                };
            }
            catch 
            {

                return new AddressValidatorResponce
                {
                    IsValid = false
                };
            }
        }
    }
}
