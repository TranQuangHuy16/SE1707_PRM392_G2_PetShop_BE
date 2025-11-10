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
            if (request.IsDefault == true)
            {
                var addressDefault = await _userAddressRepository.GetDefaultByUserId(request.UserId);
                addressDefault.IsDefault = false;
                await _userAddressRepository.UpdateAsync(addressDefault);
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

        public async Task<IEnumerable<UserAddressResponse>> GetByUserIdAsync(int userId)
        {
            var addresses = await _userAddressRepository.GetByUserIdAsync(userId);
            return addresses.Select(MapToResponse).ToList();
        }

        // Update UserAddress
        public async Task<UserAddressResponse> UpdateAsync(int id, UserAddressRequest request)
        {
            var existing = await _userAddressRepository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Address not found.");

            if(request.IsDefault == true)
            {
                var addressDefault = await _userAddressRepository.GetDefaultByUserId(existing.UserId);
                if (addressDefault != null && addressDefault.AddressId != existing.AddressId)
                {
                    addressDefault.IsDefault = false;
                    await _userAddressRepository.UpdateAsync(addressDefault);
                }
            }

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

        public async Task<UserAddressResponse> GetAdmin()
        {
            var addr = await _userAddressRepository.GetAdmin();
            return MapToResponse(addr);
        }

        public async Task<UserAddressResponse> RemoveAsync(int id)
        {
            var existing = await _userAddressRepository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Address not found.");

            await _userAddressRepository.RemoveAsync(existing);
            return MapToResponse(existing);
        }
    }

}
