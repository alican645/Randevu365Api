using Randevu365.Domain.Base;

namespace Randevu365.Domain.Entities;

public class AppUserInformation : BaseEntity
{
    public String Name { get; set; }
    public String Surname { get; set; }
    public String PhoneNumber { get; set; }
}