namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentsByUserId;

public class GetCommentsByUserIdQueryResponse
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public string? BusinessName { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
