using PRN232_SU25_SE182614_Repository;
using PRN232_SU25_SE182614_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_SU25_SE182614_Service
{
    public class SystemAccountService
    {
        private readonly SystemAccountRepository _repo;
        public SystemAccountService()
        {
            _repo ??= new SystemAccountRepository();
        }
        public SystemAccountService(SystemAccountRepository repo)  {
            _repo = repo;

        }

        public async Task<SystemAccount> Login(string email, string password)
        {
            return await _repo.GetAccount(email, password);
        }
    }
}
