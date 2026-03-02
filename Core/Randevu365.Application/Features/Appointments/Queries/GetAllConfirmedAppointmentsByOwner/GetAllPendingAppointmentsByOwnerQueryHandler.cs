using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Queries.GetAllPendingAppointmentsByOwner;

public class GetAllConfirmedAppointmentsByOwnerQueryHandler : IRequestHandler<GetAllConfirmedAppointmentsByOwnerQueryRequest, ApiResponse<GetAllConfirmedAppointmentsByOwnerQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetAllConfirmedAppointmentsByOwnerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetAllConfirmedAppointmentsByOwnerQueryResponse>> Handle(GetAllConfirmedAppointmentsByOwnerQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<GetAllConfirmedAppointmentsByOwnerQueryResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var userId = _currentUserService.UserId.Value;

        var businessIds = (await _unitOfWork.GetReadRepository<Business>().GetAllAsync(
            predicate: b => b.AppUserId == userId && !b.IsDeleted
        )).Select(b => b.Id).ToList();

        if (!businessIds.Any())
        {
            return ApiResponse<GetAllConfirmedAppointmentsByOwnerQueryResponse>.NotFoundResult("Size ait işletme bulunamadı.");
        }

        var appointments = await _unitOfWork.GetReadRepository<Appointment>().GetAllAsync(
            predicate: a => businessIds.Contains(a.BusinessId) && a.Status == AppointmentStatus.Confirmed && !a.IsDeleted,
            include: q => q
                .Include(a => a.AppUser).ThenInclude(u => u!.AppUserInformation)
                .Include(a => a.BusinessService)
                .Include(a => a.Business),
            orderBy: q => q.OrderBy(a => a.AppointmentDate).ThenBy(a => a.RequestedStartTime)
        );

        var responseItems = appointments.Select(a => new GetAllConfirmedAppointmentsByOwnerQueryResponseItem
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

        var response = new GetAllConfirmedAppointmentsByOwnerQueryResponse
        {
            Items = responseItems
        };

        return ApiResponse<GetAllConfirmedAppointmentsByOwnerQueryResponse>.SuccessResult(response);
    }
}
