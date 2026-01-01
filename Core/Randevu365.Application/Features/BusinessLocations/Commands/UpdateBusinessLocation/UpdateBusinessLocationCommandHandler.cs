using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessLocations.Commands.UpdateBusinessLocation;

public class UpdateBusinessLocationCommandHandler : IRequestHandler<UpdateBusinessLocationCommandRequest, ApiResponse<UpdateBusinessLocationCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusinessLocationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<UpdateBusinessLocationCommandResponse>> Handle(UpdateBusinessLocationCommandRequest request, CancellationToken cancellationToken)
    {
        var location = await _unitOfWork.GetReadRepository<BusinessLocation>().GetAsync(x => x.Id == request.Id);

        if (location == null)
        {
            return ApiResponse<UpdateBusinessLocationCommandResponse>.NotFoundResult("Business location not found.");
        }

        location.Latitude = request.Latitude;
        location.Longitude = request.Longitude;

        await _unitOfWork.GetWriteRepository<BusinessLocation>().UpdateAsync(location);
        await _unitOfWork.SaveAsync();

        return ApiResponse<UpdateBusinessLocationCommandResponse>.SuccessResult(new UpdateBusinessLocationCommandResponse { Id = location.Id }, "Business location updated successfully.");
    }
}
