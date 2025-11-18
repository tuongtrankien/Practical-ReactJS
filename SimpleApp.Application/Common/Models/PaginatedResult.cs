namespace SimpleApp.Application.Common.Models;

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public PaginatedResult() { }

    public PaginatedResult(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }
}
