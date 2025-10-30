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
    public class PaymentMapper : Profile
    {
        public PaymentMapper()
        {
            CreateMap<Payment, PaymentResponse>()
                .ForMember(dest => dest.PaymentMethodName,
                           opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentStatusName,
                           opt => opt.MapFrom(src => src.PaymentStatus.ToString()));

            CreateMap<CreatePaymentRequest, Payment>();
        }
    }
}
