using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Requests
{
    public class SendMessageRequest
    {
        public int? ChatRoomId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
    }
}
