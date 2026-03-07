namespace Randevu365.Application.Features.Favorites.Queries.GetMyFavorites;

public class GetMyFavoritesQueryResponse
{
    public int FavoriteId { get; set; }
    public int BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessCity { get; set; } = string.Empty;
    public string? BusinessCategory { get; set; }
    public DateTime AddedAt { get; set; }
}
