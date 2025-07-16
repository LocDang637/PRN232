using SmokeQuit.Services.LocDPX;
using SmokeQuit.SoapAPIServices.LocDPX.SoapModels;
using System.ServiceModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmokeQuit.SoapAPIServices.LocDPX.SoapServices
{
    [ServiceContract]
    public interface IChatsLocDpxSoapService
    {
        [OperationContract]
        Task<List<ChatsLocDpx>> GetAllAsync();
        [OperationContract]
        Task<ChatsLocDpx> GetByIdAsync(int userId);
        [OperationContract]
        Task<int> CreateAsync(ChatsLocDpx chat);
        [OperationContract]
        Task<ChatsLocDpx> UpdateAsync(ChatsLocDpx chat);
        [OperationContract]
        Task<int> DeleteAsync(int chatId);

    }
    public class ChatsLocDpxSoapService : IChatsLocDpxSoapService
    {
        private readonly IServiceProviders _serviceProviders;

        public ChatsLocDpxSoapService(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders ?? throw new ArgumentNullException(nameof(serviceProviders));
        }
        public async Task<int> CreateAsync(ChatsLocDpx chat)
        {
            try
            {
                // Manual mapping from SOAP model to Repository model
                var repoChat = new SmokeQuit.Repository.LocDPX.Models.ChatsLocDpx
                {
                    UserId = chat.UserId,
                    CoachId = chat.CoachId,
                    Message = chat.Message,
                    SentBy = chat.SentBy,
                    MessageType = chat.MessageType,
                    IsRead = chat.IsRead,
                    AttachmentUrl = chat.AttachmentUrl,
                    ResponseTime = chat.ResponseTime,
                    CreatedAt = chat.CreatedAt ?? DateTime.Now
                };

                var result = await _serviceProviders.ChatsService.CreateAsync(repoChat);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteAsync(int chatId)
        {
            try
            {
                var result = await _serviceProviders.ChatsService.DeleteAsync(chatId);
                return result ? 1 : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                return 0;
            }
        }

        //public async Task<List<ChatsLocDpx>> GetAllAsync()
        //{
        //    try
        //    {
        //        var chats = await _serviceProviders.ChatsService.GetAllAsync();

        //        var opt = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        //        var cashDepositSlipJsonString = JsonSerializer.Serialize(chats, opt);

        //        var result = JsonSerializer.Deserialize<List<ChatsLocDpx>>(cashDepositSlipJsonString, opt);

        //        return result;
        //    }
        //    catch (Exception ex) { }

        //    return new List<ChatsLocDpx>();
        //}


        public async Task<List<ChatsLocDpx>> GetAllAsync()
        {
            try
            {
                // Get data from service
                var chats = await _serviceProviders.ChatsService.SearchAsync(null, null, null);

                // Manual mapping to avoid circular reference issues
                var result = chats.Select(chat => new ChatsLocDpx
                {
                    ChatsLocDpxid = chat.ChatsLocDpxid,
                    UserId = chat.UserId,
                    CoachId = chat.CoachId,
                    Message = chat.Message,
                    SentBy = chat.SentBy,
                    MessageType = chat.MessageType,
                    IsRead = chat.IsRead,
                    AttachmentUrl = chat.AttachmentUrl,
                    ResponseTime = chat.ResponseTime,
                    CreatedAt = chat.CreatedAt
                    // Don't include Coach and User navigation properties
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return new List<ChatsLocDpx>();
            }
        }

        public async Task<ChatsLocDpx> GetByIdAsync(int userId)
        {
            try
            {
                var chat = await _serviceProviders.ChatsService.GetGetByIdAsync(userId);

                if (chat == null)
                    return new ChatsLocDpx();

                var result = new ChatsLocDpx
                {
                    ChatsLocDpxid = chat.ChatsLocDpxid,
                    UserId = chat.UserId,
                    CoachId = chat.CoachId,
                    Message = chat.Message,
                    SentBy = chat.SentBy,
                    MessageType = chat.MessageType,
                    IsRead = chat.IsRead,
                    AttachmentUrl = chat.AttachmentUrl,
                    ResponseTime = chat.ResponseTime,
                    CreatedAt = chat.CreatedAt
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                return new ChatsLocDpx();
            }
        }

        public async Task<ChatsLocDpx> UpdateAsync(ChatsLocDpx chat)
        {
            try
            {
                // Manual mapping from SOAP model to Repository model
                var repoChat = new SmokeQuit.Repository.LocDPX.Models.ChatsLocDpx
                {
                    ChatsLocDpxid = chat.ChatsLocDpxid,
                    UserId = chat.UserId,
                    CoachId = chat.CoachId,
                    Message = chat.Message,
                    SentBy = chat.SentBy,
                    MessageType = chat.MessageType,
                    IsRead = chat.IsRead,
                    AttachmentUrl = chat.AttachmentUrl,
                    ResponseTime = chat.ResponseTime,
                    CreatedAt = chat.CreatedAt
                };

                var result = await _serviceProviders.ChatsService.UpdateAsync(repoChat);

                if (result > 0)
                {
                    // Return updated chat
                    return await GetByIdAsync(chat.ChatsLocDpxid);
                }

                return new ChatsLocDpx();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                return new ChatsLocDpx();
            }
        }
    }
}
