// Enhanced CoachesLocDpxService.cs - Direct Service Implementation
using SmokeQuit.Repository.LocDPX;
using SmokeQuit.Repository.LocDPX.Models;
using SmokeQuit.Repository.LocDPX.ModelExtensions;

namespace SmokeQuit.Services.LocDPX
{
    public class CoachesLocDpxService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoachesLocDpxService() => _unitOfWork ??= new UnitOfWork();

        public async Task<List<CoachesLocDpx>> GetAllAsync()
        {
            try
            {
                return await _unitOfWork.CoachesLocDpxRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all coaches: {ex.Message}", ex);
            }
        }

        public async Task<CoachesLocDpx?> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Coach ID must be greater than 0", nameof(id));

                return await _unitOfWork.CoachesLocDpxRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting coach by ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<CoachesLocDpx?> GetByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be null or empty", nameof(email));

                var coaches = await GetAllAsync();
                return coaches.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting coach by email: {ex.Message}", ex);
            }
        }

        public async Task<List<CoachesLocDpx>> SearchAsync(string fullName, string email)
        {
            try
            {
                var coaches = await GetAllAsync();
                var query = coaches.AsQueryable();

                if (!string.IsNullOrWhiteSpace(fullName))
                    query = query.Where(c => c.FullName.Contains(fullName, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(email))
                    query = query.Where(c => c.Email.Contains(email, StringComparison.OrdinalIgnoreCase));

                return query.OrderBy(c => c.FullName).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching coaches: {ex.Message}", ex);
            }
        }

        public async Task<PaginationResult<CoachesLocDpx>> SearchWithPagingAsync(string fullName, string email, int currentPage, int pageSize)
        {
            try
            {
                if (currentPage <= 0)
                    throw new ArgumentException("Current page must be greater than 0", nameof(currentPage));

                if (pageSize <= 0 || pageSize > 100)
                    throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

                var coaches = await SearchAsync(fullName, email);
                var totalItems = coaches.Count;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var pagedCoaches = coaches
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PaginationResult<CoachesLocDpx>
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    Items = pagedCoaches
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching coaches with pagination: {ex.Message}", ex);
            }
        }

        public async Task<PaginationResult<CoachesLocDpx>> GetAllWithPagingAsync(int currentPage, int pageSize)
        {
            try
            {
                if (currentPage <= 0)
                    throw new ArgumentException("Current page must be greater than 0", nameof(currentPage));

                if (pageSize <= 0 || pageSize > 100)
                    throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

                var coaches = await GetAllAsync();
                var totalItems = coaches.Count;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var pagedCoaches = coaches
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PaginationResult<CoachesLocDpx>
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    Items = pagedCoaches
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting coaches with pagination: {ex.Message}", ex);
            }
        }

        public async Task<int> CreateAsync(CoachesLocDpx coach)
        {
            try
            {
                if (coach == null)
                    throw new ArgumentNullException(nameof(coach), "Coach cannot be null");

                if (string.IsNullOrWhiteSpace(coach.FullName))
                    throw new ArgumentException("Full name is required", nameof(coach.FullName));

                if (string.IsNullOrWhiteSpace(coach.Email))
                    throw new ArgumentException("Email is required", nameof(coach.Email));

                // Check if email already exists
                var existingCoach = await GetByEmailAsync(coach.Email);
                if (existingCoach != null)
                    throw new InvalidOperationException("A coach with this email already exists");

                coach.CreatedAt = DateTime.UtcNow;
                return await _unitOfWork.CoachesLocDpxRepository.CreateAsync(coach);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating coach: {ex.Message}", ex);
            }
        }

        public async Task<int> UpdateAsync(CoachesLocDpx coach)
        {
            try
            {
                if (coach == null)
                    throw new ArgumentNullException(nameof(coach), "Coach cannot be null");

                if (coach.CoachesLocDpxid <= 0)
                    throw new ArgumentException("Invalid coach ID", nameof(coach.CoachesLocDpxid));

                if (string.IsNullOrWhiteSpace(coach.FullName))
                    throw new ArgumentException("Full name is required", nameof(coach.FullName));

                if (string.IsNullOrWhiteSpace(coach.Email))
                    throw new ArgumentException("Email is required", nameof(coach.Email));

                // Check if email already exists (excluding current coach)
                var existingCoach = await GetByEmailAsync(coach.Email);
                if (existingCoach != null && existingCoach.CoachesLocDpxid != coach.CoachesLocDpxid)
                    throw new InvalidOperationException("A coach with this email already exists");

                return await _unitOfWork.CoachesLocDpxRepository.UpdateAsync(coach);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating coach: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Coach ID must be greater than 0", nameof(id));

                var coach = await GetByIdAsync(id);
                if (coach == null)
                    return false;

                return await _unitOfWork.CoachesLocDpxRepository.RemoveAsync(coach);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting coach with ID {id}: {ex.Message}", ex);
            }
        }
    }
}