using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessRating.Commands.UpdateBusinessRating;

public class UpdateBusinessRatingCommandRequest : IRequest<ApiResponse<UpdateBusinessRatingCommandResponse>>
{
    public int RatingId { get; set; }
    public int Rating { get; set; }
    // AppUserId JWT'den alınarak yetki kontrolü yapılacak
}
