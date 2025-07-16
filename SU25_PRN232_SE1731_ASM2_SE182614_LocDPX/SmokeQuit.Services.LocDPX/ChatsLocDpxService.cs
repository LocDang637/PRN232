using SmokeQuit.Repository.LocDPX;
using SmokeQuit.Repository.LocDPX.ModelExtensions;
using SmokeQuit.Repository.LocDPX.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.Services.LocDPX
{
    public class ChatsLocDpxService : IChatsLocDpxService
    {
        private readonly ChatsLocDpxRepository _chatsLocDpxRepository;
        public ChatsLocDpxService() => _chatsLocDpxRepository ??= new ChatsLocDpxRepository();

        public async Task<int> CreateAsync(ChatsLocDpx item)
        {
            return await _chatsLocDpxRepository.CreateAsync(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _chatsLocDpxRepository.GetByIdAsync(id);
            return await _chatsLocDpxRepository.RemoveAsync(entity);
        }

        public async Task<ChatsLocDpx> GetGetByIdAsync(int id)
        {
            return await _chatsLocDpxRepository.GetByIdAsync(id);
        }

        public async Task<PaginationResult<ChatsLocDpx>> GetAllAsync(int? currentPage = 1, int? pageSize = 0)
        {
            return await _chatsLocDpxRepository.GetAllAsyncWithPagination(currentPage: (int)currentPage, pageSize: (int)pageSize);
        }

        public async Task<List<ChatsLocDpx>> SearchAsync(string? MessageType, string? SentBy, bool? IsRead)
        {

            return await _chatsLocDpxRepository.SearchAsync(MessageType: MessageType, SentBy: SentBy, IsRead: IsRead);
        }

        public async Task<PaginationResult<ChatsLocDpx>> SearchAsyncWithPagination(string? MessageType, string? SentBy, bool? IsRead, int currentPage, int pageSize)
        {

            var list = await _chatsLocDpxRepository.SearchAsync(MessageType: MessageType, SentBy: SentBy, IsRead: IsRead);
            var totalItems = list.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            var result = new PaginationResult<ChatsLocDpx>
            {
                CurrentPage = currentPage,
                Items = list,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
            return result ?? new PaginationResult<ChatsLocDpx>();
        }



        public async Task<int> UpdateAsync(ChatsLocDpx chat)
        {
            var existingItem = await _chatsLocDpxRepository.GetByIdAsync(chat.ChatsLocDpxid);
            existingItem.CoachId = chat.CoachId;
            existingItem.UserId = chat.UserId;
            existingItem.Message = chat.Message;
            existingItem.SentBy = chat.SentBy;
            existingItem.MessageType = chat.MessageType;
            existingItem.IsRead = chat.IsRead;
            existingItem.AttachmentUrl = chat.AttachmentUrl;
            existingItem.ResponseTime = chat.ResponseTime;
            existingItem.CreatedAt = DateTime.Now;


            return await _chatsLocDpxRepository.UpdateAsync(existingItem);
        }

        public async Task<PaginationResult<ChatsLocDpx>> SearchAsyncWithPagination(ClassSearchChatRequest request)
        {
            var list = await _chatsLocDpxRepository.SearchAsync(MessageType: request.MessageType, SentBy: request.SentBy, IsRead: request.IsRead);
            var totalItem = list.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / (request.PageSize > 0 ? request.PageSize : 1));
            if (request.PageSize > 0)
            {
                list = list.Skip(((int)request.CurrentPage - 1) * (int)request.PageSize).Take((int)request.PageSize).ToList();

            }
            return new PaginationResult<ChatsLocDpx>
            {
                CurrentPage = request.CurrentPage,
                PageSize = request.PageSize,
                Items = list,
                TotalItems = totalItem,
                TotalPages = totalPages,
            };
        }
    }
}