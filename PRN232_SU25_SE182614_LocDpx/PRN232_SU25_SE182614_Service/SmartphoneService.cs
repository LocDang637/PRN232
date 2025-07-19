using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PRN232_SU25_SE182614_Repository;
using PRN232_SU25_SE182614_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_SU25_SE182614_Service
{
    public interface ISmartphoneService
    {
        Task<List<Smartphone>> GetAllAsync();
        Task<Smartphone?> GetByIdAsync(int id);
        Task<int> CreateAsync(Smartphone smartphone);
        Task<int> UpdateAsync(Smartphone smartphone);
        Task<bool> DeleteAsync(Smartphone smartphone);
        Task<object> SearchAsync(string? modelName = null, string? storage = null);
    }
    public class SmartphoneService : ISmartphoneService
    {
        private readonly SmartphoneRepository _repo;

        public SmartphoneService()
        {
            _repo ??= new SmartphoneRepository();
        }

        public SmartphoneService(SmartphoneRepository repo)
        {
            _repo = repo;
        }
        public async Task<int> CreateAsync(Smartphone smartphone)
        {
            return await _repo.CreateAsync(smartphone);
        }

        public async Task<bool> DeleteAsync(Smartphone smartphone)
        {
            return await _repo.RemoveAsync(smartphone);
        }

        public async Task<List<Smartphone>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Smartphone?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<object> SearchAsync(string? modelName = null, string? storage = null)
        {
            var smartphones = await _repo.GetAllAsync();
            if (!string.IsNullOrEmpty(modelName))
            {
                smartphones = smartphones.Where(h => h.ModelName.Contains(modelName,StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(modelName))
            {
                smartphones = smartphones.Where(h => h.Storage.Contains(storage, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var groupedResult = smartphones.GroupBy(x => x.Brand.BrandName)
                .Select(x => new
                {
                    BrandName = x.Key,
                    smartphones = x.Select(y => new
                    {
                        y.SmartphoneId,
                        y.ModelName,
                        y.Storage,
                        y.BrandId,
                        y.Color,
                        y.ReleaseDate,
                        y.Stock,
                        Brand = new
                        {
                            y.Brand?.BrandId,
                            y.Brand?.BrandName,
                            y.Brand?.Country,
                            y.Brand?.FoundedYear,
                            y.Brand?.Website,
                        }

                    })
                }).ToList();
            return groupedResult;
        }

        public async Task<int> UpdateAsync(Smartphone smartphone)
        {
            return await _repo.UpdateAsync(smartphone);
        }
    }
}
