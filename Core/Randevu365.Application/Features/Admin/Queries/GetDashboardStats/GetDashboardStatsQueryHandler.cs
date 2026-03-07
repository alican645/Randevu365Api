using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQueryRequest, ApiResponse<GetDashboardStatsQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetDashboardStatsQueryResponse>> Handle(GetDashboardStatsQueryRequest request, CancellationToken cancellationToken)
    {
        var totalUsers = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>().CountAsync(u => !u.IsDeleted);
        var totalBusinesses = await _unitOfWork.GetReadRepository<Business>().CountAsync(b => !b.IsDeleted);
        var totalAppointments = await _unitOfWork.GetReadRepository<Appointment>().CountAsync(a => !a.IsDeleted);
        var pendingAppointments = await _unitOfWork.GetReadRepository<Appointment>().CountAsync(a => a.Status == AppointmentStatus.Pending && !a.IsDeleted);
        var completedAppointments = await _unitOfWork.GetReadRepository<Appointment>().CountAsync(a => a.Status == AppointmentStatus.Completed && !a.IsDeleted);
        var totalComments = await _unitOfWork.GetReadRepository<BusinessComment>().CountAsync(c => !c.IsDeleted);

        return ApiResponse<GetDashboardStatsQueryResponse>.SuccessResult(new GetDashboardStatsQueryResponse
        {
            TotalUsers = totalUsers,
            TotalBusinesses = totalBusinesses,
            TotalAppointments = totalAppointments,
            PendingAppointments = pendingAppointments,
            CompletedAppointments = completedAppointments,
            TotalComments = totalComments
        });
    }
}
