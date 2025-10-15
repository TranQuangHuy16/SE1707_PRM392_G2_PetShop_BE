using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Models
{
    public class Otp
    {
        public int OtpId { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation
        public User? User { get; set; }
    }
}
