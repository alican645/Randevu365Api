using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Queries.GetSlotPrice;

public class GetSlotPriceQueryResponse
{
    public List<SlotPackageDto> Packages { get; set; } = new();
}

public class SlotPackageDto
{
    public SlotPackageType PackageType { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal PricePerSlot { get; set; }
}
