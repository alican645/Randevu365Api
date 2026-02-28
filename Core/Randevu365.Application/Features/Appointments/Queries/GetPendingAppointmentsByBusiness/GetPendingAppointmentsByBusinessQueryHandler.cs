using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Queries.GetPendingAppointmentsByBusiness;

public class GetPendingAppointmentsByBusinessQueryHandler : IRequestHandler<GetPendingAppointmentsByBusinessQueryRequest, ApiResponse<GetPendingAppointmentsByBusinessQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetPendingAppointmentsByBusinessQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetPendingAppointmentsByBusinessQueryResponse>> Handle(GetPendingAppointmentsByBusinessQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<GetPendingAppointmentsByBusinessQueryResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var userId = _currentUserService.UserId.Value;

        var business = await _unitOfWork.GetReadRepository<Business>().GetAsync(
            predicate: b => b.Id == request.BusinessId && !b.IsDeleted
        );

        if (business is null)
        {
            return ApiResponse<GetPendingAppointmentsByBusinessQueryResponse>.NotFoundResult("İşletme bulunamadı.");
        }

        if (business.AppUserId != userId)
        {
            return ApiResponse<GetPendingAppointmentsByBusinessQueryResponse>.ForbiddenResult("Bu işletme size ait değil.");
        }

        var appointments = await _unitOfWork.GetReadRepository<Appointment>().GetAllAsync(
            predicate: a => a.BusinessId == request.BusinessId && a.Status == AppointmentStatus.Pending && !a.IsDeleted,
            include: q => q
                .Include(a => a.AppUser).ThenInclude(u => u!.AppUserInformation)
                .Include(a => a.BusinessService)
                .Include(a => a.Business),
            orderBy: q => q.OrderBy(a => a.AppointmentDate).ThenBy(a => a.RequestedStartTime)
        );

        var responseItems = appointments.Select(a => new GetPendingAppointmentsByBusinessQueryResponseItem
        {
            AppointmentId = a.Id,
            AppointmentDate = a.AppointmentDate,
            RequestedStartTime = a.RequestedStartTime,
            RequestedEndTime = a.RequestedEndTime,
            Status = a.Status.ToString(),
            CustomerNotes = a.CustomerNotes,
            CustomerName = a.AppUser?.AppUserInformation?.Name,
            CustomerSurname = a.AppUser?.AppUserInformation?.Surname,
            CustomerEmail = a.AppUser?.Email,
            CustomerPhone = a.AppUser?.AppUserInformation?.PhoneNumber,
            ServiceTitle = a.BusinessService?.ServiceTitle,
            ServiceContent = a.BusinessService?.ServiceContent,
            ServicePrice = a.BusinessService?.ServicePrice ?? 0,
            BusinessId = a.BusinessId,
            BusinessName = a.Business?.BusinessName,
            CreatedAt = a.CreatedAt
        }).ToList();

        var response = new GetPendingAppointmentsByBusinessQueryResponse
        {
            Items = responseItems
        };

        return ApiResponse<GetPendingAppointmentsByBusinessQueryResponse>.SuccessResult(response);
    }
}
