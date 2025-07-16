using SmokeQuit.Repository.LocDPX;
using SmokeQuit.Repository.LocDPX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.Services.LocDPX
{
    public class CoachesLocDpxService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoachesLocDpxService() => _unitOfWork ??= new UnitOfWork();


        public async Task<List<CoachesLocDpx>> GetAllAsync()
        {
            return await _unitOfWork.CoachesLocDpxRepository.GetAllAsync();
        }

    }
}