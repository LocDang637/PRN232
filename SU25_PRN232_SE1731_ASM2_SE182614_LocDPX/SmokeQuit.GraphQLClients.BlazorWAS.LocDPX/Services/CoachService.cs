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
                    coachesWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
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
            var result = await _graphQLService.QueryAsync<CoachesWithPagingResponse>(query, variables);
            return result?.CoachesWithPaging ?? new PaginationResult<CoachesLocDpx>();
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
            var result = await _graphQLService.QueryAsync<SearchCoachesWithPagingResponse>(query, variables);
            return result?.SearchCoachesWithPaging ?? new PaginationResult<CoachesLocDpx>();
        }

        // Get all coaches for dropdowns (simple list)
        public async Task<List<CoachesLocDpx>> GetAllCoachesAsync()
        {
            var query = @"
                query GetAllCoaches {
                    coachesLocDpxes {
                        coachesLocDpxid
                        fullName
                        email
                        phoneNumber
                        bio
                        createdAt
                    }
                }";

            var result = await _graphQLService.QueryAsync<AllCoachesResponse>(query);
            return result?.CoachesLocDpxes ?? new List<CoachesLocDpx>();
        }

        public async Task<int> CreateCoachAsync(CoachesLocDpxInput input)
        {
            var mutation = @"
                mutation CreateCoach($createCoachInput: CoachesLocDpxInput!) {
                    createCoachesLocDpx(createCoachInput: $createCoachInput)
                }";

            var variables = new { createCoachInput = input };
            var result = await _graphQLService.QueryAsync<CreateCoachResponse>(mutation, variables);
            return result?.CreateCoachesLocDpx ?? 0;
        }

        public async Task<int> UpdateCoachAsync(CoachesLocDpxUpdateInput input)
        {
            var mutation = @"
                mutation UpdateCoach($updateCoachInput: CoachesLocDpxUpdateInput!) {
                    updateCoachesLocDpx(updateCoachInput: $updateCoachInput)
                }";

            var variables = new { updateCoachInput = input };
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

    // Response classes
    public class CoachesWithPagingResponse
    {
        public PaginationResult<CoachesLocDpx> CoachesWithPaging { get; set; } = new();
    }

    public class SearchCoachesWithPagingResponse
    {
        public PaginationResult<CoachesLocDpx> SearchCoachesWithPaging { get; set; } = new();
    }

    public class AllCoachesResponse
    {
        public List<CoachesLocDpx> CoachesLocDpxes { get; set; } = new();
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