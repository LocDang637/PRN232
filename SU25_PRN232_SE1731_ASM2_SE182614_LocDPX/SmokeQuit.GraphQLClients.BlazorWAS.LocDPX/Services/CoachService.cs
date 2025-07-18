using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services
{
    public class CoachService
    {
        private readonly GraphQLService _graphQLService;

        public CoachService(GraphQLService graphQLService)
        {
            _graphQLService = graphQLService;
        }

        public async Task<PaginationResult<CoachesLocDpx>> GetCoachesWithPagingAsync(int currentPage = 1, int pageSize = 10)
        {
            var query = @"
                query GetCoachesWithPaging($currentPage: Int!, $pageSize: Int!) {
                    getCoachesWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
                        totalItems
                        totalPages
                        currentPage
                        pageSize
                        items {
                            coachesLocDpxid
                            fullName
                            email
                            phoneNumber
                            bio
                            createdAt
                        }
                        hasPreviousPage
                        hasNextPage
                    }
                }";

            var variables = new { currentPage, pageSize };
            var result = await _graphQLService.QueryAsync<CoachesResponse>(query, variables);
            return result?.GetCoachesWithPaging ?? new PaginationResult<CoachesLocDpx>();
        }

        public async Task<PaginationResult<CoachesLocDpx>> SearchCoachesAsync(string? fullName, string? email, int currentPage = 1, int pageSize = 10)
        {
            var query = @"
                query SearchCoachesWithPaging($fullName: String, $email: String, $currentPage: Int!, $pageSize: Int!) {
                    searchCoachesWithPaging(fullName: $fullName, email: $email, currentPage: $currentPage, pageSize: $pageSize) {
                        totalItems
                        totalPages
                        currentPage
                        pageSize
                        items {
                            coachesLocDpxid
                            fullName
                            email
                            phoneNumber
                            bio
                            createdAt
                        }
                        hasPreviousPage
                        hasNextPage
                    }
                }";

            var variables = new { fullName, email, currentPage, pageSize };
            var result = await _graphQLService.QueryAsync<SearchCoachesResponse>(query, variables);
            return result?.SearchCoachesWithPaging ?? new PaginationResult<CoachesLocDpx>();
        }

        public async Task<int> CreateCoachAsync(CoachesLocDpxInput input)
        {
            var mutation = @"
                mutation CreateCoach($input: CoachesLocDpxInput!) {
                    createCoachesLocDpx(createCoachInput: $input)
                }";

            var variables = new { input };
            var result = await _graphQLService.QueryAsync<CreateCoachResponse>(mutation, variables);
            return result?.CreateCoachesLocDpx ?? 0;
        }

        public async Task<int> UpdateCoachAsync(CoachesLocDpxUpdateInput input)
        {
            var mutation = @"
                mutation UpdateCoach($input: CoachesLocDpxUpdateInput!) {
                    updateCoachesLocDpx(updateCoachInput: $input)
                }";

            var variables = new { input };
            var result = await _graphQLService.QueryAsync<UpdateCoachResponse>(mutation, variables);
            return result?.UpdateCoachesLocDpx ?? 0;
        }

        public async Task<bool> DeleteCoachAsync(int id)
        {
            var mutation = @"
                mutation DeleteCoach($id: Int!) {
                    deleteCoachesLocDpx(id: $id)
                }";

            var variables = new { id };
            var result = await _graphQLService.QueryAsync<DeleteCoachResponse>(mutation, variables);
            return result?.DeleteCoachesLocDpx ?? false;
        }
    }

    public class CoachesResponse
    {
        public PaginationResult<CoachesLocDpx> GetCoachesWithPaging { get; set; } = new();
    }

    public class SearchCoachesResponse
    {
        public PaginationResult<CoachesLocDpx> SearchCoachesWithPaging { get; set; } = new();
    }

    public class CreateCoachResponse
    {
        public int CreateCoachesLocDpx { get; set; }
    }

    public class UpdateCoachResponse
    {
        public int UpdateCoachesLocDpx { get; set; }
    }

    public class DeleteCoachResponse
    {
        public bool DeleteCoachesLocDpx { get; set; }
    }
}