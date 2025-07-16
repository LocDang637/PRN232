using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokeQuit.APIServices.BE.LocDPX.Dto;
using SmokeQuit.Repositories.LocDPX.Models;
using SmokeQuit.Repositories.LocDPX.ModelExtensions;
using SmokeQuit.Services.LocDPX;

namespace SmokeQuit.APIServices.BE.LocDPX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsLocDpxController : ControllerBase
    {
        private readonly IChatsLocDpxService _service;

        public ChatsLocDpxController(IChatsLocDpxService service)
        {
            _service = service;
        }

        // GET: api/ChatsLocDpx
        [HttpGet]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var chats = await _service.GetAllAsync();
                return Ok(chats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/ChatsLocDpx/5
        [HttpGet("{id}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid chat ID");

                var chat = await _service.GetByIdAsync(id);
                if (chat == null)
                    return NotFound($"Chat with ID {id} not found");

                return Ok(chat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/ChatsLocDpx
        [HttpPost]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Create([FromBody] ChatDto chatDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validate required fields
                if (chatDto.UserId <= 0)
                    return BadRequest("Valid User ID is required");

                if (chatDto.CoachId <= 0)
                    return BadRequest("Valid Coach ID is required");

                if (string.IsNullOrWhiteSpace(chatDto.Message))
                    return BadRequest("Message is required");

                var chat = new ChatsLocDpx
                {
                    Message = chatDto.Message.Trim(),
                    MessageType = chatDto.MessageType ?? "text",
                    UserId = chatDto.UserId,
                    CoachId = chatDto.CoachId,
                    SentBy = chatDto.SentBy ?? "user",
                    AttachmentUrl = chatDto.AttachmentUrl,
                    IsRead = chatDto.IsRead,
                    ResponseTime = chatDto.ResponseTime,
                    CreatedAt = DateTime.UtcNow // Ensure UTC time
                };

                var result = await _service.CreateAsync(chat);
                return CreatedAtAction(nameof(GetById), new { id = result }, chat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating chat: {ex.Message}");
            }
        }

        // PUT: api/ChatsLocDpx/5
        [HttpPut("{id}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Update(int id, [FromBody] ChatDto chatDto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid chat ID");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingChat = await _service.GetByIdAsync(id);
                if (existingChat == null)
                    return NotFound($"Chat with ID {id} not found");

                // Update only allowed fields
                existingChat.Message = chatDto.Message?.Trim() ?? existingChat.Message;
                existingChat.MessageType = chatDto.MessageType ?? existingChat.MessageType;
                existingChat.SentBy = chatDto.SentBy ?? existingChat.SentBy;
                existingChat.AttachmentUrl = chatDto.AttachmentUrl;
                existingChat.IsRead = chatDto.IsRead;
                existingChat.ResponseTime = chatDto.ResponseTime;
                // Note: Don't update CreatedAt, UserId, CoachId for data integrity

                await _service.UpdateAsync(existingChat);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating chat: {ex.Message}");
            }
        }

        // DELETE: api/ChatsLocDpx/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "1")] // Only admin can delete
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid chat ID");

                var result = await _service.DeleteAsync(id);
                if (!result)
                    return NotFound($"Chat with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting chat: {ex.Message}");
            }
        }

        // POST: api/ChatsLocDpx/search
        [HttpPost("Search")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Search([FromBody] SearchChatRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Search request is required");

                // Set defaults for pagination
                request.CurrentPage = request.CurrentPage <= 0 ? 1 : request.CurrentPage;
                request.PageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 100); // Max 100 items

                var result = await _service.SearchWithPagingAsync(
                    request.Message ?? "",
                    request.MessageType ?? "",
                    request.SentBy ?? "",
                    request.StartDate,
                    request.EndDate,
                    request.CurrentPage,
                    request.PageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error searching chats: {ex.Message}");
            }
        }

        // GET: api/ChatsLocDpx/paging/{currentPage}/{pageSize}
        [HttpGet("paging/{currentPage}/{pageSize}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetWithPaging(int currentPage, int pageSize)
        {
            try
            {
                if (currentPage <= 0)
                    return BadRequest("Current page must be greater than 0");

                if (pageSize <= 0 || pageSize > 100)
                    return BadRequest("Page size must be between 1 and 100");

                var result = await _service.GetAllWithPagingAsync(currentPage, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting paged chats: {ex.Message}");
            }
        }

        // GET: api/ChatsLocDpx/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetChatsByUser(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest("Invalid user ID");

                var chats = await _service.GetChatsByUserAsync(userId);
                return Ok(chats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting user chats: {ex.Message}");
            }
        }

        // GET: api/ChatsLocDpx/coach/{coachId}
        [HttpGet("coach/{coachId}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetChatsByCoach(int coachId)
        {
            try
            {
                if (coachId <= 0)
                    return BadRequest("Invalid coach ID");

                var chats = await _service.GetChatsByCoachAsync(coachId);
                return Ok(chats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting coach chats: {ex.Message}");
            }
        }
    }
}