using SmokeQuit.Repositories.LocDPX.ModelExtensions;
using SmokeQuit.Repositories.LocDPX.Models;

namespace SmokeQuit.Services.LocDPX
{
    public interface IChatsLocDpxService
    {
        Task<List<ChatsLocDpx>> GetAllAsync();
        Task<ChatsLocDpx> GetByIdAsync(int id);
        Task<ChatsLocDpx> GetByIdForUpdateAsync(int id); // Added for updates with tracking
        Task<List<ChatsLocDpx>> SearchAsync(string message, string messageType, string sentBy, DateTime? startDate, DateTime? endDate);
        Task<PaginationResult<List<ChatsLocDpx>>> SearchWithPagingAsync(string message, string messageType, string sentBy, DateTime? startDate, DateTime? endDate, int currentPage, int pageSize);
        Task<PaginationResult<List<ChatsLocDpx>>> GetAllWithPagingAsync(int currentPage, int pageSize);
        Task<List<ChatsLocDpx>> GetChatsByUserAsync(int userId);
        Task<List<ChatsLocDpx>> GetChatsByCoachAsync(int coachId);
        Task<int> CreateAsync(ChatsLocDpx input);
        Task<int> UpdateAsync(ChatsLocDpx input);
        Task<bool> DeleteAsync(int id);
    }
}