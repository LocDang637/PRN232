namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models
{
    public class PaginationResult<T> where T : class
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; } = new List<T>();
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int StartIndex => (CurrentPage - 1) * PageSize + 1;
        public int EndIndex => Math.Min(CurrentPage * PageSize, TotalItems);
    }
}
