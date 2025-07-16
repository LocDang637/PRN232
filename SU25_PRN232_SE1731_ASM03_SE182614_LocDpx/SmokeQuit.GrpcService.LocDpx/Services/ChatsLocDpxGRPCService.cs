using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SmokeQuit.GrpcService.LocDpx.Protos;
using SmokeQuit.Repository.LocDPX.Models;
using SmokeQuit.Services.LocDPX;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmokeQuit.GrpcService.LocDpx.Services
{
    public class ChatsLocDpxGRPCService : ChatsLocDpxGRPC.ChatsLocDpxGRPCBase
    {
        private readonly IServiceProviders _serviceProviders;

        public ChatsLocDpxGRPCService(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders;
        }

        public override async Task<ChatsLocDpxList> GetAllAsync(EmptyRequest request, ServerCallContext context)
        {
            var result = new ChatsLocDpxList();
            try
            {
                var chats = await _serviceProviders.ChatsService.GetAllAsync();

                // Manual mapping from domain model to protobuf model
                foreach (var chat in chats.Items) // Assuming GetAllAsync returns PaginationResult
                {
                    var protoChat = new SmokeQuit.GrpcService.LocDpx.Protos.ChatsLocDpx
                    {
                        ChatsLocDpxid = chat.ChatsLocDpxid,
                        UserId = chat.UserId,
                        CoachId = chat.CoachId,
                        Message = chat.Message ?? "",
                        SentBy = chat.SentBy ?? "",
                        MessageType = chat.MessageType ?? "",
                        IsRead = chat.IsRead,
                        AttachmentUrl = chat.AttachmentUrl ?? "",
                        ResponseTime = chat.ResponseTime?.ToString("yyyy-MM-ddTHH:mm:ss") ?? "",
                        CreatedAt = chat.CreatedAt?.ToString("yyyy-MM-ddTHH:mm:ss") ?? ""
                    };
                    result.Items.Add(protoChat);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
            }
            return result;
        }

        public override async Task<SmokeQuit.GrpcService.LocDpx.Protos.ChatsLocDpx> GetByIdAsync(ChatsLocDpxIdRequest request, ServerCallContext context)
        {
            try
            {
                var chat = await _serviceProviders.ChatsService.GetGetByIdAsync(request.Id);

                return new SmokeQuit.GrpcService.LocDpx.Protos.ChatsLocDpx
                {
                    ChatsLocDpxid = chat.ChatsLocDpxid,
                    UserId = chat.UserId,
                    CoachId = chat.CoachId,
                    Message = chat.Message ?? "",
                    SentBy = chat.SentBy ?? "",
                    MessageType = chat.MessageType ?? "",
                    IsRead = chat.IsRead,
                    AttachmentUrl = chat.AttachmentUrl ?? "",
                    ResponseTime = chat.ResponseTime?.ToString("yyyy-MM-ddTHH:mm:ss") ?? "",
                    CreatedAt = chat.CreatedAt?.ToString("yyyy-MM-ddTHH:mm:ss") ?? ""
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                return new SmokeQuit.GrpcService.LocDpx.Protos.ChatsLocDpx();
            }
        }

        public override async Task<MutationResult> CreateAsync(SmokeQuit.GrpcService.LocDpx.Protos.ChatsLocDpx request, ServerCallContext context)
        {
            try
            {
                // Convert protobuf model to domain model
                var domainChat = new SmokeQuit.Repository.LocDPX.Models.ChatsLocDpx
                {
                    UserId = request.UserId,
                    CoachId = request.CoachId,
                    Message = request.Message,
                    SentBy = request.SentBy,
                    MessageType = request.MessageType,
                    IsRead = request.IsRead,
                    AttachmentUrl = string.IsNullOrEmpty(request.AttachmentUrl) ? null : request.AttachmentUrl,
                    ResponseTime = string.IsNullOrEmpty(request.ResponseTime) ? null : DateTime.Parse(request.ResponseTime),
                    CreatedAt = string.IsNullOrEmpty(request.CreatedAt) ? null : DateTime.Parse(request.CreatedAt)
                };

                var result = await _serviceProviders.ChatsService.CreateAsync(domainChat);
                return new MutationResult { Result = result };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAsync: {ex.Message}");
                return new MutationResult { Result = -1 };
            }
        }

        public override async Task<MutationResult> UpdateAsync(SmokeQuit.GrpcService.LocDpx.Protos.ChatsLocDpx request, ServerCallContext context)
        {
            try
            {
                // Convert protobuf model to domain model
                var domainChat = new SmokeQuit.Repository.LocDPX.Models.ChatsLocDpx
                {
                    ChatsLocDpxid = request.ChatsLocDpxid,
                    UserId = request.UserId,
                    CoachId = request.CoachId,
                    Message = request.Message,
                    SentBy = request.SentBy,
                    MessageType = request.MessageType,
                    IsRead = request.IsRead,
                    AttachmentUrl = string.IsNullOrEmpty(request.AttachmentUrl) ? null : request.AttachmentUrl,
                    ResponseTime = string.IsNullOrEmpty(request.ResponseTime) ? null : DateTime.Parse(request.ResponseTime),
                    CreatedAt = string.IsNullOrEmpty(request.CreatedAt) ? null : DateTime.Parse(request.CreatedAt)
                };

                var result = await _serviceProviders.ChatsService.UpdateAsync(domainChat);
                return new MutationResult { Result = result };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                return new MutationResult { Result = -1 };
            }
        }

        public override async Task<MutationResult> DeleteAsync(ChatsLocDpxIdRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _serviceProviders.ChatsService.DeleteAsync(request.Id);
                return new MutationResult { Result = result ? 1 : 0 };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                return new MutationResult { Result = -1 };
            }
        }
    }
}