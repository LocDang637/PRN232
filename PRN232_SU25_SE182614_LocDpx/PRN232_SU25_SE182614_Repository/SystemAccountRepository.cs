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
    public class SystemAccountRepository : GenericRepository<SystemAccount>
    {
        public SystemAccountRepository() { }
        public SystemAccountRepository(SmartphoneDbContext context)
        {
            _context = context;
        }

        public async Task<SystemAccount> GetAccount(string email, string password)
        {
            return await _context.SystemAccounts.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        }
    }
}
