using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandRequest : IRequest<ApiResponse<CreateAppointmentCommandResponse>>
{
    public int BusinessId { get; set; }
    public int BusinessServiceId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly RequestedStartTime { get; set; }
    public TimeOnly RequestedEndTime { get; set; }
    public string? CustomerNotes { get; set; }
}
