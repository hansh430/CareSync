namespace CareSync.Dtos
{
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages =>
        (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
