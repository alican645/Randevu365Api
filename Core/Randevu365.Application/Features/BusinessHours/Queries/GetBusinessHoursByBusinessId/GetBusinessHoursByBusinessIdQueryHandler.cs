using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessHours.Queries.GetBusinessHoursByBusinessId;

public class GetBusinessHoursByBusinessIdQueryHandler : IRequestHandler<GetBusinessHoursByBusinessIdQueryRequest, ApiResponse<List<GetBusinessHoursByBusinessIdQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessHoursByBusinessIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<GetBusinessHoursByBusinessIdQueryResponse>>> Handle(GetBusinessHoursByBusinessIdQueryRequest request, CancellationToken cancellationToken)
    {
        var businessHours = await _unitOfWork.GetReadRepository<BusinessHour>().GetAllAsync(x => x.BusinessId == request.BusinessId);

        var response = businessHours.Select(x => new GetBusinessHoursByBusinessIdQueryResponse
        {
            Id = x.Id,
            Day = x.Day,
            OpenTime = x.OpenTime,
            CloseTime = x.CloseTime
        }).ToList();

        return ApiResponse<List<GetBusinessHoursByBusinessIdQueryResponse>>.SuccessResult(response);
    }
}
