using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Queries.GetConfirmedAppointmentsByBusiness;

public class GetConfirmedAppointmentsByBusinessQueryHandler : IRequestHandler<GetConfirmedAppointmentsByBusinessQueryRequest, ApiResponse<GetConfirmedAppointmentsByBusinessQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetConfirmedAppointmentsByBusinessQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetConfirmedAppointmentsByBusinessQueryResponse>> Handle(GetConfirmedAppointmentsByBusinessQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<GetConfirmedAppointmentsByBusinessQueryResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var userId = _currentUserService.UserId.Value;

        var business = await _unitOfWork.GetReadRepository<Business>().GetAsync(
            predicate: b => b.Id == request.BusinessId && !b.IsDeleted
        );

        if (business is null)
        {
            return ApiResponse<GetConfirmedAppointmentsByBusinessQueryResponse>.NotFoundResult("İşletme bulunamadı.");
        }

        if (business.AppUserId != userId)
        {
            return ApiResponse<GetConfirmedAppointmentsByBusinessQueryResponse>.ForbiddenResult("Bu işletme size ait değil.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = today.AddDays(14);

        var appointments = await _unitOfWork.GetReadRepository<Appointment>().GetAllAsync(
            predicate: a => a.BusinessId == request.BusinessId
                         && (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed)
                         && !a.IsDeleted
                         && a.AppointmentDate >= today
                         && a.AppointmentDate <= endDate,
            include: q => q
                .Include(a => a.AppUser).ThenInclude(u => u!.AppUserInformation)
                .Include(a => a.BusinessService)
                .Include(a => a.Business),
            orderBy: q => q.OrderBy(a => a.AppointmentDate).ThenBy(a => a.RequestedStartTime)
        );

        var responseItems = appointments.Select(a => new GetConfirmedAppointmentsByBusinessQueryResponseItem
        {
            AppointmentId = a.Id,
            AppointmentDate = a.AppointmentDate,
            RequestedStartTime = a.RequestedStartTime,
            RequestedEndTime = a.RequestedEndTime,
            ApproveStartTime = a.ApproveStartTime,
            ApproveEndTime = a.ApproveEndTime,
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

        var response = new GetConfirmedAppointmentsByBusinessQueryResponse
        {
            Items = responseItems
        };

        return ApiResponse<GetConfirmedAppointmentsByBusinessQueryResponse>.SuccessResult(response);
    }
}
