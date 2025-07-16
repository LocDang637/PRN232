using SmokeQuit.Services.LocDPX;
using SmokeQuit.SoapAPIServices.LocDPX.SoapModels;
using System.ServiceModel;

namespace SmokeQuit.SoapAPIServices.LocDPX.SoapServices
{

    [ServiceContract]
    public interface ICoachLocDpxSoapService
    {
        [OperationContract]
        Task<List<CoachesLocDpx>> GetAllAsync();
    }

    public class CoachLocDpxSoapService : ICoachLocDpxSoapService
    {
        private readonly IServiceProviders _serviceProviders;

        public CoachLocDpxSoapService(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders ?? throw new ArgumentNullException(nameof(serviceProviders));
        }

        public async Task<List<CoachesLocDpx>> GetAllAsync()
        {
            try
            {
                var coaches = await _serviceProviders.CoachesService.GetAllAsync();

                var result = coaches.Select(coach => new CoachesLocDpx
                {
                    CoachesLocDpxid = coach.CoachesLocDpxid,
                    FullName = coach.FullName,
                    Email = coach.Email,
                    PhoneNumber = coach.PhoneNumber,
                    Bio = coach.Bio,
                    CreatedAt = coach.CreatedAt
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CoachLocDpxSoapService.GetAllAsync: {ex.Message}");
                return new List<CoachesLocDpx>();
            }
        }
    }
}
