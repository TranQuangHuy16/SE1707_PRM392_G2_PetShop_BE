using PetShop.Repositories.Models.Enums;
using System;
using System.Collections.Generic;

namespace PetShop.Repositories.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int? AddressId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;
        public bool IsActive { get; set; } = true;

        // Navigation
        public User User { get; set; }
        public UserAddress Address { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
