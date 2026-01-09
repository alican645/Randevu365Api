namespace Randevu365.Application.Features.BusinessComments.Commands.AddBusinessComment;

public class AddBusinessCommentCommandResponse
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int AppUserId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
