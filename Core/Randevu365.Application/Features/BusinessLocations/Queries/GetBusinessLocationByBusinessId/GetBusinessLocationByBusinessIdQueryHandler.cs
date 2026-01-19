using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessLocations.Queries.GetBusinessLocationByBusinessId;

public class GetBusinessLocationByBusinessIdQueryHandler : IRequestHandler<GetBusinessLocationByBusinessIdQueryRequest, ApiResponse<IList<GetBusinessLocationByBusinessIdQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessLocationByBusinessIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IList<GetBusinessLocationByBusinessIdQueryResponse>>> Handle(GetBusinessLocationByBusinessIdQueryRequest request, CancellationToken cancellationToken)
    {
        var validator = new GetBusinessLocationByBusinessIdQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<IList<GetBusinessLocationByBusinessIdQueryResponse>>.FailResult(errors);
        }

        var locations = await _unitOfWork.GetReadRepository<BusinessLocation>().GetAllAsync(x => x.BusinessId == request.BusinessId);

        var response = locations.Select(l => new GetBusinessLocationByBusinessIdQueryResponse { BusinessLocation = l }).ToList();
        return ApiResponse<IList<GetBusinessLocationByBusinessIdQueryResponse>>.SuccessResult(response);
    }
}
