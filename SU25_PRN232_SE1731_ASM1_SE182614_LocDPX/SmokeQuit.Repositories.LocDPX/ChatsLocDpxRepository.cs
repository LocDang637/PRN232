using Microsoft.EntityFrameworkCore;
using SmokeQuit.Repositories.LocDPX.Basic;
using SmokeQuit.Repositories.LocDPX.DBContext;
using SmokeQuit.Repositories.LocDPX.Models;
using SmokeQuit.Repositories.LocDPX.ModelExtensions;

namespace SmokeQuit.Repositories.LocDPX
{
    public class ChatsLocDpxRepository : GenericRepository<ChatsLocDpx>
    {
        public ChatsLocDpxRepository()
        {
            _context ??= new SU25_PRN232_SE1731_G6_SmokeQuitContext();
        }

        public ChatsLocDpxRepository(SU25_PRN232_SE1731_G6_SmokeQuitContext context)
        {
            _context = context;
        }

        // Override GetAllAsync with includes - Added 'new' keyword
        public new async Task<List<ChatsLocDpx>> GetAllAsync()
        {
            return await _context.ChatsLocDpxes
                .Include(x => x.Coach)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking() // Prevent tracking issues
                .ToListAsync();
        }

        // Override GetByIdAsync with includes - Added 'new' keyword
        public new async Task<ChatsLocDpx> GetByIdAsync(int id)
        {
            return await _context.ChatsLocDpxes
                .Include(x => x.Coach)
                .Include(x => x.User)
                .AsNoTracking() // For read operations
                .FirstOrDefaultAsync(x => x.ChatsLocDpxid == id);
        }

        // Get by ID for updates (with tracking)
        public async Task<ChatsLocDpx> GetByIdForUpdateAsync(int id)
        {
            return await _context.ChatsLocDpxes
                .Include(x => x.Coach)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ChatsLocDpxid == id);
        }

        // Search with multiple criteria
        public async Task<List<ChatsLocDpx>> SearchAsync(string message, string messageType, string sentBy, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var query = _context.ChatsLocDpxes
                    .Include(x => x.Coach)
                    .Include(x => x.User)
                    .AsQueryable();

                // Apply filters only if values are provided
                if (!string.IsNullOrWhiteSpace(message))
                    query = query.Where(x => x.Message.Contains(message));

                if (!string.IsNullOrWhiteSpace(messageType))
                    query = query.Where(x => x.MessageType.Contains(messageType));

                if (!string.IsNullOrWhiteSpace(sentBy))
                    query = query.Where(x => x.SentBy.Contains(sentBy));

                if (startDate.HasValue)
                    query = query.Where(x => x.CreatedAt >= startDate);

                if (endDate.HasValue)
                {
                    // Ensure end date includes the full day
                    var endDateTime = endDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(x => x.CreatedAt <= endDateTime);
                }

                return await query
                    .OrderByDescending(x => x.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching chats: {ex.Message}", ex);
            }
        }

        // Search with pagination - Fixed pagination logic
        public async Task<PaginationResult<List<ChatsLocDpx>>> SearchWithPagingAsync(string message, string messageType, string sentBy, DateTime? startDate, DateTime? endDate, int currentPage, int pageSize)
        {
            try
            {
                var query = _context.ChatsLocDpxes
                    .Include(x => x.Coach)
                    .Include(x => x.User)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(message))
                    query = query.Where(x => x.Message.Contains(message));

                if (!string.IsNullOrWhiteSpace(messageType))
                    query = query.Where(x => x.MessageType.Contains(messageType));

                if (!string.IsNullOrWhiteSpace(sentBy))
                    query = query.Where(x => x.SentBy.Contains(sentBy));

                if (startDate.HasValue)
                    query = query.Where(x => x.CreatedAt >= startDate);

                if (endDate.HasValue)
                {
                    var endDateTime = endDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(x => x.CreatedAt <= endDateTime);
                }

                // Get total count before pagination
                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // Apply pagination and get data
                var chats = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

                return new PaginationResult<List<ChatsLocDpx>>
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    Items = chats
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching chats with pagination: {ex.Message}", ex);
            }
        }

        // Get all with pagination - Fixed pagination logic
        public async Task<PaginationResult<List<ChatsLocDpx>>> GetAllWithPagingAsync(int currentPage, int pageSize)
        {
            try
            {
                var query = _context.ChatsLocDpxes
                    .Include(x => x.Coach)
                    .Include(x => x.User)
                    .OrderByDescending(x => x.CreatedAt);

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var chats = await query
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

                return new PaginationResult<List<ChatsLocDpx>>
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    Items = chats
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting chats with pagination: {ex.Message}", ex);
            }
        }

        // Get chats by user
        public async Task<List<ChatsLocDpx>> GetChatsByUserAsync(int userId)
        {
            try
            {
                return await _context.ChatsLocDpxes
                    .Include(x => x.Coach)
                    .Include(x => x.User)
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting chats by user: {ex.Message}", ex);
            }
        }

        // Get chats by coach
        public async Task<List<ChatsLocDpx>> GetChatsByCoachAsync(int coachId)
        {
            try
            {
                return await _context.ChatsLocDpxes
                    .Include(x => x.Coach)
                    .Include(x => x.User)
                    .Where(x => x.CoachId == coachId)
                    .OrderByDescending(x => x.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting chats by coach: {ex.Message}", ex);
            }
        }

        // Override CreateAsync to return the created entity ID
        public new async Task<int> CreateAsync(ChatsLocDpx entity)
        {
            try
            {
                entity.CreatedAt = DateTime.UtcNow; // Ensure UTC
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return entity.ChatsLocDpxid;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating chat: {ex.Message}", ex);
            }
        }

        // Override UpdateAsync with proper tracking handling
        public new async Task<int> UpdateAsync(ChatsLocDpx entity)
        {
            try
            {
                // Clear tracking to avoid conflicts
                _context.ChangeTracker.Clear();

                // Attach and mark as modified
                var tracker = _context.Attach(entity);
                tracker.State = EntityState.Modified;

                // Don't allow updates to certain fields
                tracker.Property(x => x.CreatedAt).IsModified = false;
                tracker.Property(x => x.UserId).IsModified = false;
                tracker.Property(x => x.CoachId).IsModified = false;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating chat: {ex.Message}", ex);
            }
        }
    }
}