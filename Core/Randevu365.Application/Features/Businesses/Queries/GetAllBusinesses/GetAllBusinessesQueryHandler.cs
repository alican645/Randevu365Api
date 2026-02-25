using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.DTOs;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Businesses.Queries.GetAllBusinesses;

public class GetAllBusinessesQueryHandler : IRequestHandler<GetAllBusinessesQueryRequest, ApiResponse<IList<GetAllBusinessesQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllBusinessesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IList<GetAllBusinessesQueryResponse>>> Handle(GetAllBusinessesQueryRequest request, CancellationToken cancellationToken)
    {
        var businesses = await _unitOfWork.GetReadRepository<Business>().GetAllAsync(
            include: q => q.Include(b => b.BusinessLocations)
                .Include(b => b.AppUser).ThenInclude(u => u!.AppUserInformation)
        );

        var response = businesses.Select(b => new GetAllBusinessesQueryResponse
        {
            Id = b.Id,
            BusinessName = b.BusinessName,
            BusinessAddress = b.BusinessAddress,
            BusinessCity = b.BusinessCity,
            BusinessPhone = b.BusinessPhone,
            BusinessEmail = b.BusinessEmail,
            BusinessCountry = b.BusinessCountry,
            CreatedAt = b.CreatedAt,
            AppUserId = b.AppUserId,
            OwnerName = b.AppUser?.AppUserInformation?.Name,
            BusinessLocations = b.BusinessLocations.Select(l => new BusinessLocationDto
            {
                Id = l.Id,
                Latitude = l.Latitude,
                Longitude = l.Longitude
            }).ToList()
        }).ToList();

        return ApiResponse<IList<GetAllBusinessesQueryResponse>>.SuccessResult(response);
    }
}
