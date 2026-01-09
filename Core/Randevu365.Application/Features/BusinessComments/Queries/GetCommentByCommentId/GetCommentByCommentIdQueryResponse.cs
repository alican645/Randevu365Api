namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentByCommentId;

public class GetCommentByCommentIdQueryResponse
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int AppUserId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}
