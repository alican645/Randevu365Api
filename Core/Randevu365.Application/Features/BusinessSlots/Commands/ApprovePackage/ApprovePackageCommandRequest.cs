using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessSlots.Commands.ApprovePackage;

public class ApprovePackageCommandRequest : IRequest<ApiResponse<ApprovePackageCommandResponse>>
{
    public Guid PackageId { get; set; }
    public string? Notes { get; set; }
}
