using GraphQL;
using GraphQL.Client.Abstractions;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services;
using LoginResponseModel = SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models.LoginResponse;
namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.GraphQLClients
{
    public class GraphQLConsumer
    {
        private readonly IGraphQLClient _graphQLClient;

        public GraphQLConsumer(IGraphQLClient graphQLClient) => _graphQLClient = graphQLClient;

        #region JWT Authentication Methods

        public async Task<LoginResponseModel?> LoginAsync(string username, string password)
        {
            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"mutation Login($username: String!, $password: String!) {
                login(username: $username, password: $password) {
                    token
                    user {
                        userAccountId
                        userName
                        fullName
                        email
                        phone
                        employeeCode
                        roleId
                        isActive
                        createdDate
                    }
                }
            }",
                    Variables = new { username = username, password = password }
                };

                var response = await _graphQLClient.SendMutationAsync<LoginMutationResponse>(graphQLRequest);
                return response?.Data?.login;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"query ValidateToken($token: String!) {
                        validateToken(token: $token)
                    }",
                    Variables = new { token = token }
                };

                var response = await _graphQLClient.SendQueryAsync<ValidateTokenResponse>(graphQLRequest);
                return response?.Data?.validateToken ?? false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation error: {ex.Message}");
                return false;
            }
        }

        public async Task<SystemUserAccount?> GetCurrentUserAsync()
        {
            try
            {
                var query = @"query GetCurrentUser {
                    getCurrentUser {
                        userAccountId
                        userName
                        fullName
                        email
                        phone
                        employeeCode
                        roleId
                        isActive
                        createdDate
                    }
                }";

                var response = await _graphQLClient.SendQueryAsync<CurrentUserResponse>(query);
                return response?.Data?.getCurrentUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get current user error: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region ChatsLocDpx Methods

        public async Task<List<ChatsLocDpx>> GetChatsLocDpxes()
        {
            try
            {
                var query = @"query ChatsLocDpxes {
                    chatsLocDpxes {
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
                        user {
                            userAccountId
                            userName
                            fullName
                            email
                        }
                    }
                }";

                var response = await _graphQLClient.SendQueryAsync<ChatsLocDpxesResponse>(query);
                var result = response?.Data?.chatsLocDpxes;
                return result?.OrderByDescending(x => x.ChatsLocDpxid).ToList() ?? new List<ChatsLocDpx>();
            }
            catch (Exception ex)
            {
                return new List<ChatsLocDpx>();
            }
        }

        public async Task<ChatsLocDpx> GetChatsLocDpx(int id)
        {
            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"query ChatsLocDpxById($id: Int!) {
                        chatsLocDpxById(id: $id) {
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
                            user {
                                userAccountId
                                userName
                                fullName
                                email
                            }
                        }
                    }",
                    Variables = new { id = id }
                };

                var response = await _graphQLClient.SendQueryAsync<ChatsLocDpxResponse>(graphQLRequest);
                var result = response?.Data?.chatsLocDpxById ?? new ChatsLocDpx();
                return result;
            }
            catch (Exception ex)
            {
                return new ChatsLocDpx();
            }
        }

        public async Task<int> CreateChatsLocDpx(ChatsLocDpx chatsLocDpx)
        {
            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"mutation createChatsLocDpx ($input: ChatsLocDpxInput!) {
                        createChatsLocDpx (createChatsLocDpxInput: $input)
                    }",
                    Variables = new { input = chatsLocDpx }
                };

                var response = await _graphQLClient.SendMutationAsync<CreateChatsLocDpxResponse>(graphQLRequest);
                var result = response.Data.createChatsLocDpx;
                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<int> UpdateChatsLocDpx(ChatsLocDpx chatsLocDpx)
        {
            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"mutation updateChatsLocDpx ($input: ChatsLocDpxInput!) {
                        updateChatsLocDpx (updateChatsLocDpxInput: $input)
                    }",
                    Variables = new { input = chatsLocDpx }
                };

                var response = await _graphQLClient.SendMutationAsync<UpdateChatsLocDpxResponse>(graphQLRequest);
                var result = response.Data.updateChatsLocDpx;
                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<bool> DeleteChatsLocDpx(int id)
        {
            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"mutation deleteChatsLocDpx($input: Int!) {
                        deleteChatsLocDpx (id: $input)
                    }",
                    Variables = new { input = id }
                };

                var response = await _graphQLClient.SendMutationAsync<DeleteChatsLocDpxResponse>(graphQLRequest);
                var result = response.Data.deleteChatsLocDpx;
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region CoachesLocDpx Methods - Only GetAll

        public async Task<List<CoachesLocDpx>> GetCoachesLocDpxes()
        {
            try
            {
                var query = @"query CoachesLocDpxes {
                    coachesLocDpxes {
                        coachesLocDpxid
                        fullName
                        email
                        phoneNumber
                        bio
                        createdAt
                    }
                }";

                var response = await _graphQLClient.SendQueryAsync<CoachesLocDpxesResponse>(query);
                var result = response?.Data?.coachesLocDpxes;
                return result?.OrderByDescending(x => x.CoachesLocDpxid).ToList() ?? new List<CoachesLocDpx>();
            }
            catch (Exception ex)
            {
                return new List<CoachesLocDpx>();
            }
        }

        #endregion

        #region SystemUserAccount Methods - Only GetUserAccount (login) and GetAll

        public async Task<SystemUserAccount?> GetUserAccount(string username, string password)
        {
            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"query GetUserAccount($username: String!, $password: String!) {
                        getUserAccount(username: $username, password: $password) {
                            userAccountId
                            userName
                            password
                            fullName
                            email
                            phone
                            employeeCode
                            roleId
                            requestCode
                            createdDate
                            applicationCode
                            createdBy
                            modifiedDate
                            modifiedBy
                            isActive
                        }
                    }",
                    Variables = new { username = username, password = password }
                };

                var response = await _graphQLClient.SendQueryAsync<SystemUserAccountResponse>(graphQLRequest);
                var result = response?.Data?.getUserAccount; // Fixed: was systemUserAccountById, now getUserAccount
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<SystemUserAccount>> GetSystemUserAccounts()
        {
            try
            {
                var query = @"query SystemUserAccounts {
                    systemUserAccounts {
                        userAccountId
                        userName
                        password
                        fullName
                        email
                        phone
                        employeeCode
                        roleId
                        requestCode
                        createdDate
                        applicationCode
                        createdBy
                        modifiedDate
                        modifiedBy
                        isActive
                    }
                }";

                var response = await _graphQLClient.SendQueryAsync<SystemUserAccountsResponse>(query);
                var result = response?.Data?.systemUserAccounts;
                return result?.OrderByDescending(x => x.UserAccountId).ToList() ?? new List<SystemUserAccount>();
            }
            catch (Exception ex)
            {
                return new List<SystemUserAccount>();
            }
        }

        #endregion

        #region Pagination Methods - Only for ChatsLocDpx

        public async Task<PaginationResult<ChatsLocDpx>> GetChatsLocDpxesWithPagination(ClassSearchChatsRequestInput request)
        {
            try
            {
                var query = @"query SearchChatsWithPaging($request: ClassSearchChatsRequestInput!) {
                    searchChatsWithPaging(request: $request) {
                        totalPages
                        currentPage
                        pageSize
                        totalItems
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
                            user {
                                userAccountId
                                userName
                                fullName
                                email
                            }
                        }
                    }
                }"; // Fixed: Proper GraphQL variable syntax

                var response = await _graphQLClient.SendQueryAsync<ChatsLocDpxesWithPaginationResponse>(query);
                var result = response?.Data?.searchChatsWithPaging;

                if (result?.items != null)
                {
                    result.items = result.items.OrderByDescending(x => x.ChatsLocDpxid).ToList();
                }

                return result ?? new PaginationResult<ChatsLocDpx>
                {
                    totalPages = 0,
                    currentPage = request.CurrentPage ?? 1,
                    pageSize = request.PageSize ?? 10,
                    totalItems = 0,
                    items = new List<ChatsLocDpx>()
                };
            }
            catch (Exception ex)
            {
                return new PaginationResult<ChatsLocDpx>
                {
                    totalPages = 0,
                    currentPage = request.CurrentPage ?? 1,
                    pageSize = request.PageSize ?? 10,
                    totalItems = 0,
                    items = new List<ChatsLocDpx>()
                };
            }
        }

        #endregion
    }
}