using Randevu365.Domain.Base;
using Randevu365.Domain.Enum;

namespace Randevu365.Domain.Entities;

public class AppUser(string email, string password , Roles role) : BaseEntity
{
    public String Email { get; set; } = email;
    public String Password { get; set; } = password;
    public Roles Role { get; set; }
    public int? AppUserInformationId { get; set; }
    public virtual AppUserInformation? AppUserInformation { get; set; }
    public virtual ICollection<Business>? Businesses { get; set; } = new List<Business>();
}