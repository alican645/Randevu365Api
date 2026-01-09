using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessRating.Commands.AddBusinessRating;

public class AddBusinessRatingCommandRequest : IRequest<ApiResponse<AddBusinessRatingCommandResponse>>
{
    public int BusinessId { get; set; }
    public int Rating { get; set; }
    // AppUserId JWT'den alınacak
}
