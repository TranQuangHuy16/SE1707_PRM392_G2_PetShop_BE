using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Repositories.Models;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;

namespace PetShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("create-room")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateChatRoomRequest dto)
        {
            var room = await _chatService.CreateChatRoomAsync(dto);
            return Ok(room);
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetRoomById(int customerId)
        {
            var room = await _chatService.GetChatRoomByIdAsync(customerId);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var success = await _chatService.DeleteChatRoomAsync(id);
            if (!success) return NotFound();
            return Ok(new { Deleted = true });
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] ReceiveMessageResponse response)
        {
            var messageId = await _chatService.SendMessageAsync(response);
            return Ok(new { MessageId = messageId });
        }

        [HttpGet("messages/{chatRoomId}")]
        public async Task<IActionResult> GetMessages(int chatRoomId)
        {
            var messages = await _chatService.GetMessagesAsync(chatRoomId);
            return Ok(messages);
        }
    }
}
