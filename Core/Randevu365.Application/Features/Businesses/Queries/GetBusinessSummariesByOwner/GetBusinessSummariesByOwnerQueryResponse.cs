namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessSummariesByOwner;

public class GetBusinessSummariesByOwnerQueryResponseItem
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string? BusinessCategory { get; set; }
    public string? LogoUrl { get; set; }
    public int TodayPendingCount { get; set; }
    public int TodayConfirmedCount { get; set; }
    public int TotalPendingCount { get; set; }
}

public class GetBusinessSummariesByOwnerQueryResponse
{
    public IList<GetBusinessSummariesByOwnerQueryResponseItem> Businesses { get; set; }
}