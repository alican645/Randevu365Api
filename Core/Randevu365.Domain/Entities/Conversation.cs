namespace Randevu365.Domain.Entities;


using Randevu365.Domain.Base;

public class Conversation : BaseEntity
{
    public int UserId { get; set; }
    public int OtherUserId { get; set; }
    public string ConversationId { get; set; } = string.Empty;
    public int? AppointmentId { get; set; }
    public bool IsClosed { get; set; } = false;
    public virtual Appointment? Appointment { get; set; }
}
