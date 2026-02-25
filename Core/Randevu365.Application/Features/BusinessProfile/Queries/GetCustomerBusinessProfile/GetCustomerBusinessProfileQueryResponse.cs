namespace Randevu365.Application.Features.BusinessProfile.Queries.GetCustomerBusinessProfile;

public class GetCustomerBusinessProfileQueryResponse
{
    public int BusinessId { get; set; }
    public required string BusinessName { get; set; }
    public required string BusinessAddress { get; set; }
    public required string BusinessCity { get; set; }
    public required string BusinessPhone { get; set; }
    public required string BusinessEmail { get; set; }
    public string? BusinessCategory { get; set; }
    public string? LogoUrl { get; set; }
    public CustomerBusinessLocationDto? Location { get; set; }
    public List<CustomerBusinessServiceDto> Services { get; set; } = new();
    public List<CustomerBusinessHourDto> BusinessHours { get; set; } = new();
    public List<CustomerBusinessPhotoDto> Photos { get; set; } = new();
    public decimal AverageRating { get; set; }
    public int CommentCount { get; set; }
}

public class CustomerBusinessLocationDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

public class CustomerBusinessServiceDto
{
    public required int Id { get; set; }
    public required string ServiceTitle { get; set; }
    public required string ServiceContent { get; set; }
    public decimal ServicePrice { get; set; }
    public int MaxConcurrentCustomers { get; set; }
}

public class CustomerBusinessHourDto
{
    public required string Day { get; set; }
    public required string OpenTime { get; set; }
    public required string CloseTime { get; set; }
}

public class CustomerBusinessPhotoDto
{
    public string PhotoPath { get; set; } = string.Empty;
}

