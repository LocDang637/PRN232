using SmokeQuit.Repository.LocDPX.Basic;
using SmokeQuit.Repository.LocDPX.DBContext;
using SmokeQuit.Repository.LocDPX.ModelExtensions;
using SmokeQuit.Repository.LocDPX.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.Repository.LocDPX
{
    public class ChatsLocDpxRepository : GenericRepository<ChatsLocDpx>
    {
        public ChatsLocDpxRepository() => _context ??= new SmokeQuitDbContext();

        public ChatsLocDpxRepository(SmokeQuitDbContext context) => _context = context;

        // Lấy toàn bộ lịch sử chat và include thông tin coach
        public async Task<List<ChatsLocDpx>> GetAllAsync()
        {
            var chats = await _context.ChatsLocDpxes
                .Include(v => v.Coach).OrderByDescending(x => x.ChatsLocDpxid)
                .ToListAsync();

            return chats ?? new List<ChatsLocDpx>();
        }

        // Lấy một chat theo ID
        public async Task<ChatsLocDpx> GetByIdAsync(int id)
        {
            var chat = await _context.ChatsLocDpxes
                .Include(v => v.Coach).AsNoTracking()
                .FirstOrDefaultAsync(v => v.ChatsLocDpxid == id);

            return chat ?? new ChatsLocDpx();
        }

        public async Task<int> CreateAsync(ChatsLocDpx entity)
        {
            try
            {
                var item = await _context.ChatsLocDpxes.OrderByDescending(x => x.ChatsLocDpxid).FirstOrDefaultAsync();
                var id = (item != null) ? item.ChatsLocDpxid + 1 : 1;
                entity.ChatsLocDpxid = id;
                _context.Add(entity);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Tìm kiếm theo 3 điều kiện: MessageType, SentBy, IsRead (cho phép null)
        public async Task<List<ChatsLocDpx>> SearchAsync(string? MessageType, string? SentBy, bool? IsRead)
        {
            var query = _context.ChatsLocDpxes
       .Include(v => v.Coach)
       .OrderByDescending(x => x.ChatsLocDpxid)
       .AsQueryable();

            if (!string.IsNullOrWhiteSpace(MessageType))
            {
                query = query.Where(d => d.MessageType != null && d.MessageType.ToLower().Equals(MessageType.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(SentBy))
            {
                query = query.Where(v => v.SentBy != null && v.SentBy.ToLower().Equals(SentBy.ToLower()));
            }

            if (IsRead.HasValue)
            {
                query = query.Where(d => d.IsRead == IsRead.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<PaginationResult<ChatsLocDpx>> GetAllAsyncWithPagination(int currentPage, int pageSize)
        {
            var chats = _context.ChatsLocDpxes
                .Include(v => v.Coach).OrderByDescending(x => x.ChatsLocDpxid)
                .AsQueryable();

            var TotalItem = chats.Count();
            var totalPages = 1;
            if (pageSize > 0)
            {
                totalPages = (int)Math.Ceiling((double)TotalItem / pageSize);

                chats = chats.Skip((currentPage - 1) * pageSize).Take(pageSize);
            }
            var result = new PaginationResult<ChatsLocDpx>
            {

                CurrentPage = currentPage,
                Items = chats,
                PageSize = pageSize,
                TotalItems = TotalItem,
                TotalPages = totalPages
            };
            return result ?? new PaginationResult<ChatsLocDpx>();
        }

        public async Task<PaginationResult<ChatsLocDpx>> GetAllAsyncWithPagination(string? MessageType, DateTime? CreatedAt, string? SentBy, int currentPage, int pageSize)
        {
            var query = _context.ChatsLocDpxes
               .Include(v => v.Coach)
               .AsQueryable();

            if (!String.IsNullOrEmpty(MessageType))
                query = query.Where(v => v.MessageType == MessageType);

            if (CreatedAt.HasValue)
                query = query.Where(v => v.CreatedAt.Value.Date == CreatedAt.Value.Date);

            if (SentBy != null)
                query = query.Where(v => v.SentBy.Contains(SentBy));

            var TotalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)TotalItem / pageSize);
            query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);
            var result = new PaginationResult<ChatsLocDpx>
            {
                CurrentPage = currentPage,
                Items = query,
                PageSize = pageSize,
                TotalItems = TotalItem,
                TotalPages = totalPages
            };
            return result ?? new PaginationResult<ChatsLocDpx>();
        }

        public async Task<string> existSchedule(int id)
        {
            bool hasSchedules = await _context.ChatsLocDpxes
         .AnyAsync(a => a.CoachId == id);
            if (hasSchedules)
                return "Cannot delete coach because exist history remains.";
            return null;

        }
    }
}