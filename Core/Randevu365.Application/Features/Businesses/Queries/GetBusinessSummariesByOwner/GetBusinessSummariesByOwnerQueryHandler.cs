using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessSummariesByOwner;

public class GetBusinessSummariesByOwnerQueryHandler : IRequestHandler<GetBusinessSummariesByOwnerQueryRequest, ApiResponse<GetBusinessSummariesByOwnerQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetBusinessSummariesByOwnerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetBusinessSummariesByOwnerQueryResponse>> Handle(GetBusinessSummariesByOwnerQueryRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
        {
            return ApiResponse<GetBusinessSummariesByOwnerQueryResponse>.UnauthorizedResult();
        }

        var businesses = await _unitOfWork.GetReadRepository<Business>()
            .GetAllAsync(
                predicate: x => x.AppUserId == currentUserId && !x.IsDeleted,
                include: i => i
                    .Include(b => b.BusinessLogo)
                    .Include(b => b.Appointments)
            );

        var today = DateOnly.FromDateTime(DateTime.Now);

        var responseItems = businesses.Select(b => new GetBusinessSummariesByOwnerQueryResponseItem
        {
            Id = b.Id,
            BusinessName = b.BusinessName,
            BusinessAddress = b.BusinessAddress,
            BusinessCategory = b.BusinessCategory?.ToJson(),
            LogoUrl = b.BusinessLogo?.LogoUrl,
            TodayPendingCount = b.Appointments?.Count(a =>
                a.AppointmentDate == today &&
                a.Status == AppointmentStatus.Pending &&
                !a.IsDeleted) ?? 0,
            TodayConfirmedCount = b.Appointments?.Count(a =>
                a.AppointmentDate == today &&
                a.Status == AppointmentStatus.Confirmed &&
                !a.IsDeleted) ?? 0,
            TotalPendingCount = b.Appointments?.Count(a =>
                a.Status == AppointmentStatus.Pending &&
                !a.IsDeleted) ?? 0
        }).ToList();

        var response = new GetBusinessSummariesByOwnerQueryResponse {Businesses =  responseItems};

        return ApiResponse<GetBusinessSummariesByOwnerQueryResponse>.SuccessResult(response);
    }
}
