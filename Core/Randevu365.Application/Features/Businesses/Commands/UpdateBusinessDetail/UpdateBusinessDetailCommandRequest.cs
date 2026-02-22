using MediatR;
using Microsoft.AspNetCore.Http;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Commands.UpdateBusinessDetail;

public class UpdateBusinessDetailCommandRequest : IRequest<ApiResponse<UpdateBusinessDetailCommandResponse>>
{
    public string? BusinessName { get; set; }
    public string? BusinessAddress { get; set; }
    public string? BusinessCity { get; set; }
    public string? BusinessPhone { get; set; }
    public string? BusinessEmail { get; set; }
    public string? BusinessCountry { get; set; }
    public IFormFile? BusinessLogo { get; set; }
    public List<BusinessServiceUpdateDto>? BusinessServices { get; set; }
    public List<BusinessHourUpdateDto>? BusinessHours { get; set; }
    public List<IFormFile>? BusinessPhotos { get; set; }
    public List<int>? PhotoIdsToDelete { get; set; }
    public BusinessLocationDto? Location { get; set; }
}

public class BusinessLocationDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

public class BusinessHourUpdateDto
{
    public string? Day { get; set; }
    public string? OpenTime { get; set; }
    public string? CloseTime { get; set; }
}

public class BusinessServiceUpdateDto
{
    public string? ServiceTitle { get; set; }
    public string? ServiceContent { get; set; }
    public int MaxConcurrentCustomers { get; set; } = 1;
}
