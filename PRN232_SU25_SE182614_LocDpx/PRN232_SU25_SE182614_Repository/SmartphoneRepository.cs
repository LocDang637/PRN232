using Microsoft.EntityFrameworkCore;
using PRN232_SU25_SE182614_Repository.Basic;
using PRN232_SU25_SE182614_Repository.DBContext;
using PRN232_SU25_SE182614_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_SU25_SE182614_Repository
{
    public class SmartphoneRepository : GenericRepository<Smartphone>
    {
        public SmartphoneRepository() { }
        public SmartphoneRepository(SmartphoneDbContext context) {
            _context = context;
        }

        public async Task<List<Smartphone>> GetAllAsync()
        {
            return await _context.Smartphones.Include(x => x.Brand).ToListAsync();
        }

        public async Task<Smartphone?> GetByIdAsync(int id)
        {
            return await _context.Smartphones.Include(x => x.Brand).FirstOrDefaultAsync(x => x.SmartphoneId == id);
        }
    }
}
