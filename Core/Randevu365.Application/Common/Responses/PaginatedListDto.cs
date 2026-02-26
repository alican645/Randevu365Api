namespace Randevu365.Application.Common.Responses;

public class PaginatedListDto<T>
{
    public IList<T> Items { get; set; } = new List<T>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public PaginatedListDto()
    {
    }

    public PaginatedListDto(IList<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}
