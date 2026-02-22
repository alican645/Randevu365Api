using Randevu365.Domain.Base;
using Randevu365.Domain.Enum;

namespace Randevu365.Domain.Entities;

public class AppUser(string email, string password, string role) : BaseEntity
{
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
    public string Role { get; set; } = role;
    public int? AppUserInformationId { get; set; }
    public virtual AppUserInformation? AppUserInformation { get; set; }
    public virtual ICollection<Business>? Businesses { get; set; } = new List<Business>();
    public virtual ICollection<BusinessComment> BusinessComments { get; set; } = new List<BusinessComment>();
    public virtual ICollection<BusinessRating> BusinessRatings { get; set; } = new List<BusinessRating>();
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<BusinessSlot> BusinessSlots { get; set; } = new List<BusinessSlot>();
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
}