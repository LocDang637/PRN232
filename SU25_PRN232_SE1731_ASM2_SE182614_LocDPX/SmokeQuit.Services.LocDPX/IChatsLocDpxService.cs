using SmokeQuit.Repository.LocDPX.ModelExtensions;
using SmokeQuit.Repository.LocDPX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.Services.LocDPX
{
    public interface IChatsLocDpxService
    {


        Task<ChatsLocDpx> GetGetByIdAsync(int id);

        Task<List<ChatsLocDpx>> SearchAsync(string? MessageType, string? SentBy, bool? IsRead);
        Task<PaginationResult<ChatsLocDpx>> SearchAsyncWithPagination(string? MessageType, string? SentBy, bool? IsRead, int currentPage, int pageSize);
        Task<PaginationResult<ChatsLocDpx>> SearchAsyncWithPagination(ClassSearchChatRequest request);
        Task<PaginationResult<ChatsLocDpx>> GetAllAsync(int? currentPage = 1, int? pageSize = 0);

        Task<int> CreateAsync(ChatsLocDpx item);
        Task<int> UpdateAsync(ChatsLocDpx chat);
        Task<bool> DeleteAsync(int id);
    }
}