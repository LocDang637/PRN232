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

        [OperationContract]
        Task<CoachesLocDpx> GetByIdAsync(int coachId);

        [OperationContract]
        Task<int> CreateAsync(CoachesLocDpx coach);

        [OperationContract]
        Task<CoachesLocDpx> UpdateAsync(CoachesLocDpx coach);

        [OperationContract]
        Task<int> DeleteAsync(int coachId);

        [OperationContract]
        Task<List<CoachesLocDpx>> SearchAsync(string fullName, string email);
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

        public async Task<CoachesLocDpx> GetByIdAsync(int coachId)
        {
            try
            {
                if (coachId <= 0)
                {
                    Console.WriteLine("Invalid CoachId: must be greater than 0");
                    return new CoachesLocDpx();
                }

                var coach = await _serviceProviders.CoachesService.GetByIdAsync(coachId);

                if (coach == null)
                    return new CoachesLocDpx();

                var result = new CoachesLocDpx
                {
                    CoachesLocDpxid = coach.CoachesLocDpxid,
                    FullName = coach.FullName,
                    Email = coach.Email,
                    PhoneNumber = coach.PhoneNumber,
                    Bio = coach.Bio,
                    CreatedAt = coach.CreatedAt
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CoachLocDpxSoapService.GetByIdAsync: {ex.Message}");
                return new CoachesLocDpx();
            }
        }

        public async Task<int> CreateAsync(CoachesLocDpx coach)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(coach.FullName))
                {
                    Console.WriteLine("FullName cannot be empty");
                    return 0;
                }

                if (string.IsNullOrWhiteSpace(coach.Email))
                {
                    Console.WriteLine("Email cannot be empty");
                    return 0;
                }

                // Basic email validation
                if (!coach.Email.Contains("@") || !coach.Email.Contains("."))
                {
                    Console.WriteLine("Invalid email format");
                    return 0;
                }

                // Manual mapping from SOAP model to Repository model
                var repoCoach = new SmokeQuit.Repository.LocDPX.Models.CoachesLocDpx
                {
                    // DON'T set CoachesLocDpxid - let the database handle it
                    FullName = coach.FullName.Trim(),
                    Email = coach.Email.Trim().ToLower(),
                    PhoneNumber = string.IsNullOrWhiteSpace(coach.PhoneNumber) ? null : coach.PhoneNumber.Trim(),
                    Bio = string.IsNullOrWhiteSpace(coach.Bio) ? null : coach.Bio.Trim(),
                    CreatedAt = DateTime.Now // Always set to current time for new records
                };

                Console.WriteLine($"SOAP: Creating coach - Name: {repoCoach.FullName}, Email: {repoCoach.Email}");

                var result = await _serviceProviders.CoachesService.CreateAsync(repoCoach);

                Console.WriteLine($"SOAP: Coach creation result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SOAP Coach CreateAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return 0;
            }
        }

        public async Task<CoachesLocDpx> UpdateAsync(CoachesLocDpx coach)
        {
            try
            {
                // Validate required fields
                if (coach.CoachesLocDpxid <= 0)
                {
                    Console.WriteLine("Invalid CoachId: must be greater than 0");
                    return new CoachesLocDpx();
                }

                if (string.IsNullOrWhiteSpace(coach.FullName))
                {
                    Console.WriteLine("FullName cannot be empty");
                    return new CoachesLocDpx();
                }

                if (string.IsNullOrWhiteSpace(coach.Email))
                {
                    Console.WriteLine("Email cannot be empty");
                    return new CoachesLocDpx();
                }

                // Basic email validation
                if (!coach.Email.Contains("@") || !coach.Email.Contains("."))
                {
                    Console.WriteLine("Invalid email format");
                    return new CoachesLocDpx();
                }

                // Manual mapping from SOAP model to Repository model
                var repoCoach = new SmokeQuit.Repository.LocDPX.Models.CoachesLocDpx
                {
                    CoachesLocDpxid = coach.CoachesLocDpxid,
                    FullName = coach.FullName.Trim(),
                    Email = coach.Email.Trim().ToLower(),
                    PhoneNumber = string.IsNullOrWhiteSpace(coach.PhoneNumber) ? null : coach.PhoneNumber.Trim(),
                    Bio = string.IsNullOrWhiteSpace(coach.Bio) ? null : coach.Bio.Trim(),
                    CreatedAt = coach.CreatedAt // Preserve original creation date
                };

                Console.WriteLine($"SOAP: Updating coach ID: {repoCoach.CoachesLocDpxid}");

                var result = await _serviceProviders.CoachesService.UpdateAsync(repoCoach);

                if (result > 0)
                {
                    // Return updated coach
                    return await GetByIdAsync(coach.CoachesLocDpxid);
                }

                return new CoachesLocDpx();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SOAP Coach UpdateAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return new CoachesLocDpx();
            }
        }

        public async Task<int> DeleteAsync(int coachId)
        {
            try
            {
                if (coachId <= 0)
                {
                    Console.WriteLine("Invalid CoachId: must be greater than 0");
                    return 0;
                }

                Console.WriteLine($"SOAP: Deleting coach ID: {coachId}");

                var result = await _serviceProviders.CoachesService.DeleteAsync(coachId);

                Console.WriteLine($"SOAP: Coach deletion result: {(result ? "Success" : "Failed")}");
                return result ? 1 : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SOAP Coach DeleteAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return 0;
            }
        }

        public async Task<List<CoachesLocDpx>> SearchAsync(string fullName, string email)
        {
            try
            {
                Console.WriteLine($"SOAP: Searching coaches - FullName: {fullName}, Email: {email}");

                var coaches = await _serviceProviders.CoachesService.SearchAsync(fullName, email);

                var result = coaches.Select(coach => new CoachesLocDpx
                {
                    CoachesLocDpxid = coach.CoachesLocDpxid,
                    FullName = coach.FullName,
                    Email = coach.Email,
                    PhoneNumber = coach.PhoneNumber,
                    Bio = coach.Bio,
                    CreatedAt = coach.CreatedAt
                }).ToList();

                Console.WriteLine($"SOAP: Search found {result.Count} coaches");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SOAP Coach SearchAsync: {ex.Message}");
                return new List<CoachesLocDpx>();
            }
        }
    }
}