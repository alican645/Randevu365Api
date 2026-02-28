using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Commands.ApproveAppointment;

public class ApproveAppointmentCommandRequest : IRequest<ApiResponse<ApproveAppointmentCommandResponse>>
{
    public int AppointmentId { get; set; }
    public TimeOnly? ApproveStartTime { get; set; }
    public TimeOnly? ApproveEndTime { get; set; }
}
