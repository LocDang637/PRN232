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
    public class SystemUserAccountRepository : GenericRepository<SystemUserAccount>
    {
        public SystemUserAccountRepository() => _context ??= new SmokeQuitDbContext();

        public SystemUserAccountRepository(SmokeQuitDbContext context) => _context = context;

        public async Task<SystemUserAccount> GetUserAccountAsync(string user, string passwor)
        {
            //return await _context.SystemUserAccounts.FirstOrDefaultAsync(d => d.UserName == user && d.Password == passwor && d.IsActive == true);

            return await _context.SystemUserAccounts.FirstOrDefaultAsync(d => d.UserName == user && d.Password == passwor);
        }

        public async Task<SystemUserAccount> GetByIdAsync(int id)
        {
            return await _context.SystemUserAccounts.FirstOrDefaultAsync(d => d.UserAccountId == id);
        }
    }
}