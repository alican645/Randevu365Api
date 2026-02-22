namespace Randevu365.Application.Features.BusinessSlots.Commands.ApprovePackage;

public class ApprovePackageCommandResponse
{
    public Guid PackageId { get; set; }
    public int ApprovedSlotCount { get; set; }
    public DateTime PaidAt { get; set; }
}
