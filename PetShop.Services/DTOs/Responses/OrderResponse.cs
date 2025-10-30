using PetShop.Repositories.Models.Enums;
using System;
using System.Collections.Generic;

namespace PetShop.Services.DTOs.Responses
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int? AddressId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public UserAddressResponse? Address { get; set; }
        public List<OrderDetailResponse> OrderDetails { get; set; } = new List<OrderDetailResponse>();
    }

    public class OrderDetailResponse
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
