using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Requests
{
    public class CreateChatRoomRequest
    {
        public int CustomerId { get; set; }
        public int AdminId { get; set; }
    }
}
