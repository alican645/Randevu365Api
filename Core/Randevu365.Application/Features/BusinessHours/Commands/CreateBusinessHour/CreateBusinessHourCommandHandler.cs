using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessHours.Commands.CreateBusinessHour;

public class CreateBusinessHourCommandHandler : IRequestHandler<CreateBusinessHourCommandRequest, ApiResponse<CreateBusinessHourCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusinessHourCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CreateBusinessHourCommandResponse>> Handle(CreateBusinessHourCommandRequest request, CancellationToken cancellationToken)
    {
        var businessHour = new BusinessHour
        {
            Day = request.Day,
            OpenTime = request.OpenTime,
            CloseTime = request.CloseTime,
            BusinessId = request.BusinessId
        };

        await _unitOfWork.GetWriteRepository<BusinessHour>().AddAsync(businessHour);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CreateBusinessHourCommandResponse>.SuccessResult(new CreateBusinessHourCommandResponse { Id = businessHour.Id }, "Business hour created successfully.");
    }
}
