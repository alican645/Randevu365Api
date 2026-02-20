using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Appointments.Queries.GetBusinessAppointments;

public class GetBusinessAppointmentsQueryHandler : IRequestHandler<GetBusinessAppointmentsQueryRequest, ApiResponse<IList<GetBusinessAppointmentsQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetBusinessAppointmentsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<IList<GetBusinessAppointmentsQueryResponse>>> Handle(GetBusinessAppointmentsQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<IList<GetBusinessAppointmentsQueryResponse>>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(x => x.AppUserId == _currentUserService.UserId.Value && !x.IsDeleted);

        if (business == null)
        {
            return ApiResponse<IList<GetBusinessAppointmentsQueryResponse>>.NotFoundResult("İşletme bulunamadı.");
        }

        var appointments = await _unitOfWork.GetReadRepository<Appointment>().GetAllAsync(
            predicate: x => x.BusinessId == business.Id && !x.IsDeleted
                            && (request.Date == null || x.AppointmentDate == request.Date)
                            && (request.Status == null || x.Status == request.Status),
            include: q => q.Include(a => a.AppUser).Include(a => a.BusinessService)
        );

        var response = appointments.Select(a => new GetBusinessAppointmentsQueryResponse
        {
            Id = a.Id,
            AppUserId = a.AppUserId,
            ServiceTitle = a.BusinessService?.ServiceTitle,
            AppointmentDate = a.AppointmentDate,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            Status = a.Status,
            CustomerNotes = a.CustomerNotes
        }).ToList();

        return ApiResponse<IList<GetBusinessAppointmentsQueryResponse>>.SuccessResult(response);
    }
}
