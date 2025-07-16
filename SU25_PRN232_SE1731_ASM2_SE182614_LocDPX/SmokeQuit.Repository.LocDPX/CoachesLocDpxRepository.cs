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
                var item = await _context.CoachesLocDpxes.OrderByDescending(x => x.CoachesLocDpxid).FirstOrDefaultAsync();
                var id = (item != null) ? item.CoachesLocDpxid + 1 : 1;
                entity.CoachesLocDpxid = id;
                _context.Add(entity);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}