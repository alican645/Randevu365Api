using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessPhotos.Queries.GetBusinessPhotosByBusinessId;

public class GetBusinessPhotosByBusinessIdQueryHandler : IRequestHandler<GetBusinessPhotosByBusinessIdQueryRequest, ApiResponse<IList<GetBusinessPhotosByBusinessIdQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessPhotosByBusinessIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IList<GetBusinessPhotosByBusinessIdQueryResponse>>> Handle(GetBusinessPhotosByBusinessIdQueryRequest request, CancellationToken cancellationToken)
    {
        var validator = new GetBusinessPhotosByBusinessIdQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<IList<GetBusinessPhotosByBusinessIdQueryResponse>>.FailResult(errors);
        }

        var photos = await _unitOfWork.GetReadRepository<BusinessPhoto>().GetAllAsync(x => x.BusinessId == request.BusinessId);

        var response = photos.Select(p => new GetBusinessPhotosByBusinessIdQueryResponse { BusinessPhoto = p }).ToList();
        return ApiResponse<IList<GetBusinessPhotosByBusinessIdQueryResponse>>.SuccessResult(response);
    }
}
