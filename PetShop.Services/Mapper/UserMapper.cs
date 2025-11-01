using AutoMapper;
using PetShop.API.DTOs;
using PetShop.Repositories.Models;
using PetShop.Services.DTOs.Responses;

namespace PetShop.Services.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            // Mapping sang UserDetailResponseDto
            CreateMap<User, UserDetailResponseDto>()
                .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses.ToList()));

            // ? Mapping sang UserResponse (b? thi?u)
            //CreateMap<User, UserResponse>()
            //    .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            //    .ReverseMap();
        }
    }
}
