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
                // Clear any existing tracking to avoid conflicts
                _context.ChangeTracker.Clear();

                // Validate required foreign keys exist
                var userExists = await _context.SystemUserAccounts.AnyAsync(u => u.UserAccountId == entity.UserId);
                if (!userExists)
                {
                    throw new InvalidOperationException($"User with ID {entity.UserId} does not exist.");
                }

                var coachExists = await _context.CoachesLocDpxes.AnyAsync(c => c.CoachesLocDpxid == entity.CoachId);
                if (!coachExists)
                {
                    throw new InvalidOperationException($"Coach with ID {entity.CoachId} does not exist.");
                }

                // ✅ DON'T set the ID - let the database handle it since it's an IDENTITY column
                // The database will auto-generate the ID
                entity.ChatsLocDpxid = 0; // Reset to 0 to ensure EF treats it as a new entity

                // Ensure required fields are not null
                entity.Message = entity.Message ?? "";
                entity.SentBy = entity.SentBy ?? "";
                entity.MessageType = entity.MessageType ?? "";
                entity.CreatedAt = entity.CreatedAt ?? DateTime.Now;

                // Don't set navigation properties to avoid tracking issues
                entity.Coach = null;
                entity.User = null;

                _context.ChatsLocDpxes.Add(entity);
                var result = await _context.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.WriteLine($"Repository CreateAsync Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw; // Re-throw to preserve stack trace
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
                Items = chats.ToList(),
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
                Items = query.ToList(),
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