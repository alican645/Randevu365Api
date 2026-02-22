using MediatR;
using Microsoft.AspNetCore.Http;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Commands.CreateBusinessDetail;

public class CreateBusinessDetailCommandRequest : IRequest<ApiResponse<CreateBusinessDetailCommandResponse>>
{
    public string? BusinessName { get; set; }
    public string? BusinessAddress { get; set; }
    public string? BusinessCity { get; set; }
    public string? BusinessPhone { get; set; }
    public string? BusinessEmail { get; set; }
    public string? BusinessCountry { get; set; }
    public string? BusinessCategory { get; set; }
    public IFormFile? BusinessLogo { get; set; }
    public List<BusinessServiceDto>? BusinessServices { get; set; }
    public List<BusinessHourDto>? BusinessHours { get; set; }
    public List<IFormFile>? BusinessPhotos { get; set; }
}

public class BusinessHourDto
{
    public string? Day { get; set; }
    public string? OpenTime { get; set; }
    public string? CloseTime { get; set; }
}

public class BusinessServiceDto
{
    public string? ServiceTitle { get; set; }
    public string? ServiceContent { get; set; }
    public int MaxConcurrentCustomers { get; set; } = 1;
}
