using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Responses
{
    public class ReceiveMessageResponse
    {
        public int ChatRoomId { get; set; }
        public int SenderId { get; set; }
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
