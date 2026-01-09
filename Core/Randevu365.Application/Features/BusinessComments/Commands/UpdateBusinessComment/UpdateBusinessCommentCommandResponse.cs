namespace Randevu365.Application.Features.BusinessComments.Commands.UpdateBusinessComment;

public class UpdateBusinessCommentCommandResponse
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int AppUserId { get; set; }
    public string Comment { get; set; } = string.Empty;
}
