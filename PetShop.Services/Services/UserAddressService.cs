using PetShop.Repositories.Interfaces;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Services
{
    using PetShop.Repositories.Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserAddressService : IUserAddressService
    {
        private readonly IUserAddressRepository _userAddressRepository;

        public UserAddressService(IUserAddressRepository userAddressRepository)
        {
            _userAddressRepository = userAddressRepository;
        }

        // Create UserAddress
        public async Task<UserAddressResponse> CreateAsync(UserAddressRequest request)
        {
            if (request.IsDefault)
            {
                var existingAddresses = await _userAddressRepository.GetByUserIdAsync(request.UserId);
                foreach (var addr in existingAddresses.Where(a => a.IsDefault))
                {
                    addr.IsDefault = false;
                    await _userAddressRepository.UpdateAsync(addr);
                }
            }

            var newAddress = new UserAddress
            {
                UserId = request.UserId,
                AddressLine = request.AddressLine,
                City = request.City,
                District = request.District,
                Ward = request.Ward,
                PostalCode = request.PostalCode,
                IsDefault = request.IsDefault,
                Longitude = request.Longitude,
                Latitude = request.Latitude
                
            };

            await _userAddressRepository.CreateAsync(newAddress);

            return MapToResponse(newAddress);
        }
        // Get Default UserAddress
        public async Task<UserAddressResponse> GetDefaultByUserId(int userId)
        {
            var defaultAddress = await _userAddressRepository.GetDefaultByUserId(userId);
            return defaultAddress == null ? null : MapToResponse(defaultAddress);
        }
        // Update UserAddress
        public async Task<UserAddressResponse> UpdateAsync(int id, UserAddressRequest request)
        {
            var existing = await _userAddressRepository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Address not found.");

            existing.AddressLine = request.AddressLine;
            existing.City = request.City;
            existing.District = request.District;
            existing.Ward = request.Ward;
            existing.PostalCode = request.PostalCode;
            existing.IsDefault = request.IsDefault;
            existing.Longitude = request.Longitude;
            existing.Latitude = request.Latitude;

            await _userAddressRepository.UpdateAsync(existing);

            return MapToResponse(existing);
        }
        // Map UserAdress
        private static UserAddressResponse MapToResponse(UserAddress entity)
        {
            return new UserAddressResponse
            {
                AddressId = entity.AddressId,
                UserId = entity.UserId,
                AddressLine = entity.AddressLine,
                City = entity.City,
                District = entity.District,
                Ward = entity.Ward,
                PostalCode = entity.PostalCode,
                IsDefault = entity.IsDefault,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude
            };
        }
    }

}
