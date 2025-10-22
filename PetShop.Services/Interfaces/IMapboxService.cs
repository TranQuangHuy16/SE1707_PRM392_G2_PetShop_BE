using PetShop.Services.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Interfaces
{
    public interface IMapboxService
    {
        Task<CoordinatesAddressResponse> GetCoordinatesFromAddress(string address);
    }
}
