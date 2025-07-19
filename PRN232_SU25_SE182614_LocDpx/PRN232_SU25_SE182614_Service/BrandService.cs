using PRN232_SU25_SE182614_Repository;
using PRN232_SU25_SE182614_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_SU25_SE182614_Service
{
    public class BrandService
    {
        private readonly BrandRepository _repo;
        public BrandService()
        {
            _repo ??= new BrandRepository();
        }

        public BrandService(BrandRepository repo)
        {
            _repo = repo;
        }

        public async Task<Brand?> GetByIdAsync(int id) {
            return await _repo.GetByIdAsync(id);
        }
    }
}
