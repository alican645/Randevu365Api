using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Entities = Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessLogo.Queries.GetBusinessLogoByBusinessId;

public class GetBusinessLogoByBusinessIdQueryHandler : IRequestHandler<GetBusinessLogoByBusinessIdQueryRequest, ApiResponse<GetBusinessLogoByBusinessIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessLogoByBusinessIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetBusinessLogoByBusinessIdQueryResponse>> Handle(GetBusinessLogoByBusinessIdQueryRequest request, CancellationToken cancellationToken)
    {
        var businessLogo = await _unitOfWork.GetReadRepository<Entities.BusinessLogo>().GetAsync(x => x.BusinessId == request.BusinessId);

        if (businessLogo == null)
        {
            return ApiResponse<GetBusinessLogoByBusinessIdQueryResponse>.NotFoundResult("Business logo not found.");
        }

        return ApiResponse<GetBusinessLogoByBusinessIdQueryResponse>.SuccessResult(new GetBusinessLogoByBusinessIdQueryResponse
        {
            Id = businessLogo.Id,
            LogoUrl = businessLogo.LogoUrl
        });
    }
}
