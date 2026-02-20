using Randevu365.Domain.Base;
using Randevu365.Domain.Enum;

namespace Randevu365.Domain.Entities;

public class Appointment : BaseEntity
{
    public int AppUserId { get; set; }
    public virtual AppUser? AppUser { get; set; }

    public int BusinessId { get; set; }
    public virtual Business? Business { get; set; }

    public int BusinessServiceId { get; set; }
    public virtual BusinessService? BusinessService { get; set; }

    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? CustomerNotes { get; set; }
    public string? BusinessNotes { get; set; }
}
