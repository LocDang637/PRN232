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
                    chatsWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
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
            var result = await _graphQLService.QueryAsync<ChatsWithPagingResponse>(query, variables);
            return result?.ChatsWithPaging ?? new PaginationResult<ChatsLocDpx>();
        }

        // FIXED: Remove message content search, use simple search by type, sentBy, isRead
        public async Task<PaginationResult<ChatsLocDpx>> SearchChatsAsync(string? messageType, string? sentBy, bool? isRead, int currentPage = 1, int pageSize = 10)
        {
            try
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

                // Match your Postman exactly - no message content field
                var request = new
                {
                    messageType = messageType,
                    sentBy = sentBy,
                    isRead = isRead,
                    currentPage = currentPage,
                    pageSize = pageSize
                };

                var variables = new { request };
                var result = await _graphQLService.QueryAsync<SearchChatsWithPagingResponse>(query, variables);
                return result?.SearchChatsWithPaging ?? new PaginationResult<ChatsLocDpx>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search failed: {ex.Message}");
                // Fallback to regular pagination if search fails
                return await GetChatsWithPagingAsync(currentPage, pageSize);
            }
        }

        public async Task<int> CreateChatAsync(ChatsLocDpxInput input)
        {
            var mutation = @"
                mutation CreateChat($createChatsLocDpxInput: ChatsLocDpxInput!) {
                    createChatsLocDpx(createChatsLocDpxInput: $createChatsLocDpxInput)
                }";

            var variables = new { createChatsLocDpxInput = input };
            var result = await _graphQLService.QueryAsync<CreateChatResponse>(mutation, variables);
            return result?.CreateChatsLocDpx ?? 0;
        }

        public async Task<int> UpdateChatAsync(ChatsLocDpxUpdateInput input)
        {
            var mutation = @"
                mutation UpdateChat($updateChatsLocDpxInput: ChatsLocDpxUpdateInput!) {
                    updateChatsLocDpx(updateChatsLocDpxInput: $updateChatsLocDpxInput)
                }";

            var variables = new { updateChatsLocDpxInput = input };
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

    // Response classes
    public class ChatsWithPagingResponse
    {
        public PaginationResult<ChatsLocDpx> ChatsWithPaging { get; set; } = new();
    }

    public class SearchChatsWithPagingResponse
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