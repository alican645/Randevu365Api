using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Favorites.Commands.AddFavorite;

public class AddFavoriteCommandRequest : IRequest<ApiResponse<AddFavoriteCommandResponse>>
{
    public int BusinessId { get; set; }
}
