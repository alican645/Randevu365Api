using MediatR;
using Microsoft.AspNetCore.Http;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.DTOs;

namespace Randevu365.Application.Features.Businesses.Commands.UpdateBusinessDetailById;

public class UpdateBusinessDetailByIdCommandRequest : IRequest<ApiResponse<UpdateBusinessDetailByIdCommandResponse>>
{
    public int BusinessId { get; set; }
    public string? BusinessName { get; set; }
    public string? BusinessAddress { get; set; }
    public string? BusinessCity { get; set; }
    public string? BusinessPhone { get; set; }
    public string? BusinessEmail { get; set; }
    public string? BusinessCountry { get; set; }
    public string? BusinessCategory { get; set; }
    public IFormFile? BusinessLogo { get; set; }
    public List<BusinessHourDetailDto>? BusinessHours { get; set; }
    public List<BusinessServiceDetailDto>? BusinessServices { get; set; }
    public List<IFormFile>? BusinessPhotos { get; set; }
    public List<BusinessPhotoDto>? ExistingPhotos { get; set; }
    public BusinessLocationDto? Location { get; set; }
}
