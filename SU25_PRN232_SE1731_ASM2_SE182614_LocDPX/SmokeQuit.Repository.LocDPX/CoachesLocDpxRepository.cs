using SmokeQuit.Repository.LocDPX.Basic;
using SmokeQuit.Repository.LocDPX.DBContext;
using SmokeQuit.Repository.LocDPX.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.Repository.LocDPX
{
    public class CoachesLocDpxRepository : GenericRepository<CoachesLocDpx>
    {
        public CoachesLocDpxRepository() => _context ??= new SmokeQuitDbContext();

        public CoachesLocDpxRepository(SmokeQuitDbContext context) => _context = context;

        public async Task<int> CreateAsync(CoachesLocDpx entity)
        {
            try
            {
                // Clear any existing tracking to avoid conflicts
                _context.ChangeTracker.Clear();

                // ✅ DON'T set the ID - let the database handle it since it's an IDENTITY column
                // The database will auto-generate the ID
                entity.CoachesLocDpxid = 0; // Reset to 0 to ensure EF treats it as a new entity

                // Ensure required fields are not null
                entity.FullName = entity.FullName ?? "";
                entity.Email = entity.Email ?? "";
                entity.CreatedAt = entity.CreatedAt ?? DateTime.Now;

                _context.CoachesLocDpxes.Add(entity);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository CoachesLocDpx CreateAsync Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw new Exception(ex.Message);
            }
        }
    }
}