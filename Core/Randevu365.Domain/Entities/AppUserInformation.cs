using Randevu365.Domain.Base;

namespace Randevu365.Domain.Entities;

public class AppUserInformation : BaseEntity
{
    public String Name { get; set; }
    public String Surname { get; set; }
    public int Age { get; set; }
    public String Gender { get; set; }
    public String PhoneNumber { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
}