using Randevu365.Domain.Base;

namespace Randevu365.Domain.Entities;

public class BusinessLocation : BaseEntity
{
    protected BusinessLocation() { } // EF Core için

    public BusinessLocation(int businessId, decimal latitude, decimal longitude)
    {
        BusinessId = businessId;
        Latitude = latitude;
        Longitude = longitude;
    }

    public int BusinessId { get; set; }
    public virtual Business? Business { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}