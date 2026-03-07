using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Favorites.Commands.RemoveFavorite;

public class RemoveFavoriteCommandRequest : IRequest<ApiResponse>
{
    public int BusinessId { get; set; }
}
