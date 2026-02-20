namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessDetailInfoByBusinessId;

public class GetBusinessDetailInfoByBusinessIdQueryResponse
{
    public required string BusinessName { get; set; }
    public required string BusinessAddress { get; set; }
    public required string BusinessCity { get; set; }
    public required string BusinessPhone { get; set; }
    public required string BusinessEmail { get; set; }
    public required string BusinessCountry { get; set; }
    public string? BusinessLogo { get; set; }
    public List<BusinessServiceDetailDto> BusinessServices { get; set; } = new();
    public List<BusinessHourDetailDto> BusinessHours { get; set; } = new();
    public List<BusinessPhotoDto> BusinessPhotos { get; set; } = new();
}

public class BusinessPhotoDto
{
    public int Id { get; set; }
    public string PhotoPath { get; set; } = string.Empty;
}

public class BusinessHourDetailDto
{
    public required string Day { get; set; }
    public required string OpenTime { get; set; }
    public required string CloseTime { get; set; }
}

public class BusinessServiceDetailDto
{
    public required string ServiceTitle { get; set; }
    public required string ServiceContent { get; set; }
    public int MaxConcurrentCustomers { get; set; }
}
