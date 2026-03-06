using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Commands.CreateAppointmentByOwner;

public class CreateAppointmentByOwnerCommandRequest : IRequest<ApiResponse<CreateAppointmentByOwnerCommandResponse>>
{
    public int BusinessId { get; set; }
    public int BusinessServiceId { get; set; }
    public int AppUserId { get; set; } // Randevu oluşturulacak müşteri ID'si
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly RequestedStartTime { get; set; }
    public TimeOnly RequestedEndTime { get; set; }
    public string? CustomerNotes { get; set; }
    public string? BusinessNotes { get; set; }
}
