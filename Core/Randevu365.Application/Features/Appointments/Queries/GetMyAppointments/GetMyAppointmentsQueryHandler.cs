using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Appointments.Queries.GetMyAppointments;

public class GetMyAppointmentsQueryHandler : IRequestHandler<GetMyAppointmentsQueryRequest, ApiResponse<IList<GetMyAppointmentsQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyAppointmentsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<IList<GetMyAppointmentsQueryResponse>>> Handle(GetMyAppointmentsQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<IList<GetMyAppointmentsQueryResponse>>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var appointments = await _unitOfWork.GetReadRepository<Appointment>().GetAllAsync(
            predicate: x => x.AppUserId == _currentUserService.UserId.Value && !x.IsDeleted,
            include: q => q.Include(a => a.Business).Include(a => a.BusinessService)
        );

        var response = appointments.Select(a => new GetMyAppointmentsQueryResponse
        {
            Id = a.Id,
            BusinessName = a.Business?.BusinessName,
            ServiceTitle = a.BusinessService?.ServiceTitle,
            AppointmentDate = a.AppointmentDate,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            Status = a.Status
        }).ToList();

        return ApiResponse<IList<GetMyAppointmentsQueryResponse>>.SuccessResult(response);
    }
}
