using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services
{
    public class ChatService
    {
        private readonly GraphQLService _graphQLService;

        public ChatService(GraphQLService graphQLService)
        {
            _graphQLService = graphQLService;
        }

        public async Task<PaginationResult<ChatsLocDpx>> GetChatsWithPagingAsync(int currentPage = 1, int pageSize = 10)
        {
            var query = @"
                query GetChatsWithPaging($currentPage: Int!, $pageSize: Int!) {
                    getChatsWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
                        totalItems
                        totalPages
                        currentPage
                        pageSize
                        items {
                            chatsLocDpxid
                            userId
                            coachId
                            message
                            sentBy
                            messageType
                            isRead
                            attachmentUrl
                            responseTime
                            createdAt
                            coach {
                                coachesLocDpxid
                                fullName
                                email
                            }
                        }
                        hasPreviousPage
                        hasNextPage
                    }
                }";

            var variables = new { currentPage, pageSize };
            var result = await _graphQLService.QueryAsync<ChatsResponse>(query, variables);
            return result?.GetChatsWithPaging ?? new PaginationResult<ChatsLocDpx>();
        }

        public async Task<PaginationResult<ChatsLocDpx>> SearchChatsAsync(string? messageContent, string? messageType, string? sentBy, bool? isRead, int currentPage = 1, int pageSize = 10)
        {
            var query = @"
                query SearchChatsWithPaging($request: ClassSearchChatRequest!) {
                    searchChatsWithPaging(request: $request) {
                        totalItems
                        totalPages
                        currentPage
                        pageSize
                        items {
                            chatsLocDpxid
                            userId
                            coachId
                            message
                            sentBy
                            messageType
                            isRead
                            attachmentUrl
                            responseTime
                            createdAt
                            coach {
                                coachesLocDpxid
                                fullName
                                email
                            }
                        }
                        hasPreviousPage
                        hasNextPage
                    }
                }";

            var request = new
            {
                MessageType = messageType,
                SentBy = sentBy,
                IsRead = isRead,
                Message = messageContent,
                CurrentPage = currentPage,
                PageSize = pageSize
            };

            var variables = new { request };
            var result = await _graphQLService.QueryAsync<SearchChatsResponse>(query, variables);
            return result?.SearchChatsWithPaging ?? new PaginationResult<ChatsLocDpx>();
        }

        public async Task<int> CreateChatAsync(ChatsLocDpxInput input)
        {
            var mutation = @"
                mutation CreateChat($input: ChatsLocDpxInput!) {
                    createChatsLocDpx(createChatsLocDpxInput: $input)
                }";

            var variables = new { input };
            var result = await _graphQLService.QueryAsync<CreateChatResponse>(mutation, variables);
            return result?.CreateChatsLocDpx ?? 0;
        }

        public async Task<int> UpdateChatAsync(ChatsLocDpxUpdateInput input)
        {
            var mutation = @"
                mutation UpdateChat($input: ChatsLocDpxUpdateInput!) {
                    updateChatsLocDpx(updateChatsLocDpxInput: $input)
                }";

            var variables = new { input };
            var result = await _graphQLService.QueryAsync<UpdateChatResponse>(mutation, variables);
            return result?.UpdateChatsLocDpx ?? 0;
        }

        public async Task<bool> DeleteChatAsync(int id)
        {
            var mutation = @"
                mutation DeleteChat($id: Int!) {
                    deleteChatsLocDpx(id: $id)
                }";

            var variables = new { id };
            var result = await _graphQLService.QueryAsync<DeleteChatResponse>(mutation, variables);
            return result?.DeleteChatsLocDpx ?? false;
        }
    }

    public class ChatsResponse
    {
        public PaginationResult<ChatsLocDpx> GetChatsWithPaging { get; set; } = new();
    }

    public class SearchChatsResponse
    {
        public PaginationResult<ChatsLocDpx> SearchChatsWithPaging { get; set; } = new();
    }

    public class CreateChatResponse
    {
        public int CreateChatsLocDpx { get; set; }
    }

    public class UpdateChatResponse
    {
        public int UpdateChatsLocDpx { get; set; }
    }

    public class DeleteChatResponse
    {
        public bool DeleteChatsLocDpx { get; set; }
    }
}