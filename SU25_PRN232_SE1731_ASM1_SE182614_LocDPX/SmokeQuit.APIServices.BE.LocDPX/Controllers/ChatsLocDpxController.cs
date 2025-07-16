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
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _service.CreateAsync(chat);
                return CreatedAtAction(nameof(GetById), new { id = result }, chat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating chat: {ex.Message}");
            }
        }

        // PUT: api/ChatsLocDpx/5 - FIXED WITH SEPARATE UPDATE DTO
        [HttpPut("{id}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Update(int id, [FromBody] ChatUpdateDto chatUpdateDto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid chat ID");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get existing chat with proper tracking
                var existingChat = await _service.GetByIdForUpdateAsync(id);
                if (existingChat == null)
                    return NotFound($"Chat with ID {id} not found");

                // Update only the allowed fields from the UpdateDto
                existingChat.Message = chatUpdateDto.Message.Trim();
                existingChat.MessageType = chatUpdateDto.MessageType;
                existingChat.SentBy = chatUpdateDto.SentBy;
                existingChat.AttachmentUrl = chatUpdateDto.AttachmentUrl;
                existingChat.IsRead = chatUpdateDto.IsRead;
                existingChat.ResponseTime = chatUpdateDto.ResponseTime;

                // Keep original values for non-updatable fields
                // existingChat.CreatedAt - don't update
                // existingChat.UserId - don't update  
                // existingChat.CoachId - don't update
                // existingChat.ChatsLocDpxid - don't update

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

                request.CurrentPage = request.CurrentPage <= 0 ? 1 : request.CurrentPage;
                request.PageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 100);

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

        // NEW ENDPOINTS FOR ENHANCED FUNCTIONALITY

        // PATCH: api/ChatsLocDpx/{id}/mark-read
        [HttpPatch("{id}/mark-read")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid chat ID");

                var existingChat = await _service.GetByIdForUpdateAsync(id);
                if (existingChat == null)
                    return NotFound($"Chat with ID {id} not found");

                existingChat.IsRead = true;
                await _service.UpdateAsync(existingChat);

                return Ok(new { message = "Message marked as read", chatId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error marking message as read: {ex.Message}");
            }
        }

        // PATCH: api/ChatsLocDpx/{id}/mark-unread
        [HttpPatch("{id}/mark-unread")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> MarkAsUnread(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid chat ID");

                var existingChat = await _service.GetByIdForUpdateAsync(id);
                if (existingChat == null)
                    return NotFound($"Chat with ID {id} not found");

                existingChat.IsRead = false;
                await _service.UpdateAsync(existingChat);

                return Ok(new { message = "Message marked as unread", chatId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error marking message as unread: {ex.Message}");
            }
        }

        // PATCH: api/ChatsLocDpx/{id}/add-response
        [HttpPatch("{id}/add-response")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> AddResponseTime(int id, [FromBody] ResponseTimeDto responseDto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid chat ID");

                if (responseDto?.ResponseTime == null)
                    return BadRequest("Response time is required");

                var existingChat = await _service.GetByIdForUpdateAsync(id);
                if (existingChat == null)
                    return NotFound($"Chat with ID {id} not found");

                existingChat.ResponseTime = responseDto.ResponseTime;
                existingChat.IsRead = true; // Automatically mark as read when responded
                await _service.UpdateAsync(existingChat);

                return Ok(new { message = "Response time added", chatId = id, responseTime = responseDto.ResponseTime });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding response time: {ex.Message}");
            }
        }

        // GET: api/ChatsLocDpx/stats
        [HttpGet("stats")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetChatStats()
        {
            try
            {
                var allChats = await _service.GetAllAsync();

                var stats = new
                {
                    TotalMessages = allChats.Count,
                    UnreadMessages = allChats.Count(c => !c.IsRead),
                    ReadMessages = allChats.Count(c => c.IsRead),
                    TextMessages = allChats.Count(c => c.MessageType == "text"),
                    ImageMessages = allChats.Count(c => c.MessageType == "image"),
                    FileMessages = allChats.Count(c => c.MessageType == "file"),
                    UserMessages = allChats.Count(c => c.SentBy == "user"),
                    CoachMessages = allChats.Count(c => c.SentBy == "coach"),
                    MessagesWithAttachments = allChats.Count(c => !string.IsNullOrEmpty(c.AttachmentUrl)),
                    MessagesWithResponse = allChats.Count(c => c.ResponseTime.HasValue),
                    AverageResponseTimeHours = allChats
                        .Where(c => c.ResponseTime.HasValue && c.CreatedAt.HasValue)
                        .Select(c => (c.ResponseTime.Value - c.CreatedAt.Value).TotalHours)
                        .DefaultIfEmpty(0)
                        .Average(),
                    TodayMessages = allChats.Count(c => c.CreatedAt.HasValue && c.CreatedAt.Value.Date == DateTime.Today),
                    ThisWeekMessages = allChats.Count(c => c.CreatedAt.HasValue &&
                        c.CreatedAt.Value >= DateTime.Today.AddDays(-7)),
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting chat statistics: {ex.Message}");
            }
        }

        // GET: api/ChatsLocDpx/unread/count
        [HttpGet("unread/count")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var allChats = await _service.GetAllAsync();
                var unreadCount = allChats.Count(c => !c.IsRead);

                return Ok(new { unreadCount = unreadCount, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting unread count: {ex.Message}");
            }
        }

        // GET: api/ChatsLocDpx/recent/{count}
        [HttpGet("recent/{count}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetRecentChats(int count = 5)
        {
            try
            {
                if (count <= 0 || count > 50)
                    return BadRequest("Count must be between 1 and 50");

                var allChats = await _service.GetAllAsync();
                var recentChats = allChats
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(count)
                    .ToList();

                return Ok(recentChats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting recent chats: {ex.Message}");
            }
        }

        // POST: api/ChatsLocDpx/bulk-mark-read
        [HttpPost("bulk-mark-read")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> BulkMarkAsRead([FromBody] BulkUpdateDto bulkDto)
        {
            try
            {
                if (bulkDto?.ChatIds == null || !bulkDto.ChatIds.Any())
                    return BadRequest("Chat IDs are required");

                if (bulkDto.ChatIds.Count() > 100)
                    return BadRequest("Cannot update more than 100 messages at once");

                var updatedCount = 0;
                var errors = new List<string>();

                foreach (var id in bulkDto.ChatIds)
                {
                    try
                    {
                        var chat = await _service.GetByIdForUpdateAsync(id);
                        if (chat != null)
                        {
                            chat.IsRead = true;
                            await _service.UpdateAsync(chat);
                            updatedCount++;
                        }
                        else
                        {
                            errors.Add($"Chat ID {id} not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error updating chat ID {id}: {ex.Message}");
                    }
                }

                return Ok(new
                {
                    message = "Bulk update completed",
                    updatedCount = updatedCount,
                    totalRequested = bulkDto.ChatIds.Count(),
                    errors = errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error in bulk update: {ex.Message}");
            }
        }
    }
}

// Additional DTOs for new endpoints
public class ResponseTimeDto
{
    public DateTime ResponseTime { get; set; }
}

public class BulkUpdateDto
{
    public IEnumerable<int> ChatIds { get; set; }
}