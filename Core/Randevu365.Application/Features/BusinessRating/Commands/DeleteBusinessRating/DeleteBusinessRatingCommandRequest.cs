using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessRating.Commands.DeleteBusinessRating;

public class DeleteBusinessRatingCommandRequest : IRequest<ApiResponse<DeleteBusinessRatingCommandResponse>>
{
    public int RatingId { get; set; }
    // AppUserId JWT'den alınarak yetki kontrolü yapılacak
}
