using AutoMapper;
using PetShop.Repositories.Models;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Mapper
{
    public class CartMapper : Profile
    {
        public CartMapper()
        {
            CreateMap<Cart, CartResponse>()
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.TotalItems, opt => opt.Ignore());

            CreateMap<CartItem, CartItemResponse>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.ProductImageUrl,
                           opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductPrice,
                           opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.TotalPrice,
                           opt => opt.MapFrom(src => src.Product.Price * src.Quantity));
        }
    }
}
