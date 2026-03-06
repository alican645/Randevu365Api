using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Queries.GetBusinessBusySlotsByOwner;

public class GetBusinessBusySlotsByOwnerQueryHandler : IRequestHandler<GetBusinessBusySlotsByOwnerQueryRequest, ApiResponse<GetBusinessBusySlotsByOwnerQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetBusinessBusySlotsByOwnerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetBusinessBusySlotsByOwnerQueryResponse>> Handle(GetBusinessBusySlotsByOwnerQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<GetBusinessBusySlotsByOwnerQueryResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var userId = _currentUserService.UserId.Value;

        var business = await _unitOfWork.GetReadRepository<Business>().GetAsync(
            predicate: b => b.Id == request.BusinessId && !b.IsDeleted
        );

        if (business is null)
        {
            return ApiResponse<GetBusinessBusySlotsByOwnerQueryResponse>.NotFoundResult("İşletme bulunamadı.");
        }

        if (business.AppUserId != userId)
        {
            return ApiResponse<GetBusinessBusySlotsByOwnerQueryResponse>.ForbiddenResult("Bu işletme size ait değil.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = today.AddDays(14);

        // İlgili işletmenin randevularını çek (2 haftalık)
        var appointments = await _unitOfWork.GetReadRepository<Appointment>().GetAllAsync(
            predicate: a => a.BusinessId == request.BusinessId
                         && (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed)
                         && !a.IsDeleted
                         && a.AppointmentDate >= today
                         && a.AppointmentDate <= endDate,
            orderBy: q => q.OrderBy(a => a.AppointmentDate).ThenBy(a => a.ApproveStartTime ?? a.RequestedStartTime)
        );

        var response = new GetBusinessBusySlotsByOwnerQueryResponse
        {
            Items = appointments.Select(a => new GetBusinessBusySlotsByOwnerQueryResponseItem
            {
                AppointmentDate = a.AppointmentDate,
                ApproveStartTime = a.ApproveStartTime ?? a.RequestedStartTime,
                ApproveEndTime = a.ApproveEndTime ?? a.RequestedEndTime
            }).ToList()
        };

        return ApiResponse<GetBusinessBusySlotsByOwnerQueryResponse>.SuccessResult(response);
    }
}
