using SmokeQuit.Repositories.LocDPX;
using SmokeQuit.Repositories.LocDPX.Models;
using SmokeQuit.Repositories.LocDPX.ModelExtensions;

namespace SmokeQuit.Services.LocDPX
{
    public class ChatsLocDpxService : IChatsLocDpxService
    {
        private readonly ChatsLocDpxRepository _repository;

        public ChatsLocDpxService()
        {
            _repository ??= new ChatsLocDpxRepository();
        }

        public ChatsLocDpxService(ChatsLocDpxRepository repo)
        {
            _repository = repo;
        }

        public async Task<List<ChatsLocDpx>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all chats: {ex.Message}", ex);
            }
        }

        public async Task<ChatsLocDpx> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Chat ID must be greater than 0", nameof(id));

                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving chat with ID {id}: {ex.Message}", ex);
            }
        }

        // Added method for updates with tracking
        public async Task<ChatsLocDpx> GetByIdForUpdateAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Chat ID must be greater than 0", nameof(id));

                return await _repository.GetByIdForUpdateAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving chat for update with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<List<ChatsLocDpx>> SearchAsync(string message, string messageType, string sentBy, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                    throw new ArgumentException("Start date cannot be later than end date");

                return await _repository.SearchAsync(message, messageType, sentBy, startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching chats: {ex.Message}", ex);
            }
        }

        public async Task<PaginationResult<List<ChatsLocDpx>>> SearchWithPagingAsync(string message, string messageType, string sentBy, DateTime? startDate, DateTime? endDate, int currentPage, int pageSize)
        {
            try
            {
                if (currentPage <= 0)
                    throw new ArgumentException("Current page must be greater than 0", nameof(currentPage));

                if (pageSize <= 0 || pageSize > 100)
                    throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                    throw new ArgumentException("Start date cannot be later than end date");

                return await _repository.SearchWithPagingAsync(message, messageType, sentBy, startDate, endDate, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching chats with pagination: {ex.Message}", ex);
            }
        }

        public async Task<PaginationResult<List<ChatsLocDpx>>> GetAllWithPagingAsync(int currentPage, int pageSize)
        {
            try
            {
                if (currentPage <= 0)
                    throw new ArgumentException("Current page must be greater than 0", nameof(currentPage));

                if (pageSize <= 0 || pageSize > 100)
                    throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

                return await _repository.GetAllWithPagingAsync(currentPage, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paged chats: {ex.Message}", ex);
            }
        }

        public async Task<List<ChatsLocDpx>> GetChatsByUserAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("User ID must be greater than 0", nameof(userId));

                return await _repository.GetChatsByUserAsync(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving chats for user {userId}: {ex.Message}", ex);
            }
        }

        public async Task<List<ChatsLocDpx>> GetChatsByCoachAsync(int coachId)
        {
            try
            {
                if (coachId <= 0)
                    throw new ArgumentException("Coach ID must be greater than 0", nameof(coachId));

                return await _repository.GetChatsByCoachAsync(coachId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving chats for coach {coachId}: {ex.Message}", ex);
            }
        }

        public async Task<int> CreateAsync(ChatsLocDpx input)
        {
            try
            {
                if (input == null)
                    throw new ArgumentNullException(nameof(input), "Chat input cannot be null");

                if (string.IsNullOrWhiteSpace(input.Message))
                    throw new ArgumentException("Message is required", nameof(input.Message));

                if (input.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required", nameof(input.UserId));

                if (input.CoachId <= 0)
                    throw new ArgumentException("Valid Coach ID is required", nameof(input.CoachId));

                if (string.IsNullOrWhiteSpace(input.SentBy))
                    throw new ArgumentException("SentBy is required", nameof(input.SentBy));

                if (string.IsNullOrWhiteSpace(input.MessageType))
                    throw new ArgumentException("MessageType is required", nameof(input.MessageType));

                input.CreatedAt = DateTime.UtcNow;
                input.Message = input.Message.Trim();
                input.MessageType = input.MessageType.Trim().ToLower();
                input.SentBy = input.SentBy.Trim().ToLower();

                var validTypes = new[] { "text", "image", "file" };
                if (!validTypes.Contains(input.MessageType))
                    throw new ArgumentException("Invalid message type. Must be: text, image, or file", nameof(input.MessageType));

                var validSenders = new[] { "user", "coach" };
                if (!validSenders.Contains(input.SentBy))
                    throw new ArgumentException("Invalid sender. Must be: user or coach", nameof(input.SentBy));

                return await _repository.CreateAsync(input);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating chat: {ex.Message}", ex);
            }
        }

        public async Task<int> UpdateAsync(ChatsLocDpx input)
        {
            try
            {
                if (input == null)
                    throw new ArgumentNullException(nameof(input), "Chat input cannot be null");

                if (input.ChatsLocDpxid <= 0)
                    throw new ArgumentException("Invalid Chat ID", nameof(input.ChatsLocDpxid));

                if (string.IsNullOrWhiteSpace(input.Message))
                    throw new ArgumentException("Message is required", nameof(input.Message));

                if (string.IsNullOrWhiteSpace(input.MessageType))
                    throw new ArgumentException("MessageType is required", nameof(input.MessageType));

                if (string.IsNullOrWhiteSpace(input.SentBy))
                    throw new ArgumentException("SentBy is required", nameof(input.SentBy));

                input.Message = input.Message.Trim();
                input.MessageType = input.MessageType.Trim().ToLower();
                input.SentBy = input.SentBy.Trim().ToLower();

                var validTypes = new[] { "text", "image", "file" };
                if (!validTypes.Contains(input.MessageType))
                    throw new ArgumentException("Invalid message type. Must be: text, image, or file", nameof(input.MessageType));

                var validSenders = new[] { "user", "coach" };
                if (!validSenders.Contains(input.SentBy))
                    throw new ArgumentException("Invalid sender. Must be: user or coach", nameof(input.SentBy));

                return await _repository.UpdateAsync(input);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating chat: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Chat ID must be greater than 0", nameof(id));

                var chat = await _repository.GetByIdAsync(id);
                if (chat == null)
                    return false;

                return await _repository.RemoveAsync(chat);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting chat with ID {id}: {ex.Message}", ex);
            }
        }
    }
}