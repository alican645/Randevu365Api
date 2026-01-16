using FluentValidation;

namespace Randevu365.Application.Features.BusinessComments.Commands.DeleteBusinessComment;

public class DeleteBusinessCommentCommandValidator : AbstractValidator<DeleteBusinessCommentCommandRequest>
{
    public DeleteBusinessCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .GreaterThan(0).WithMessage("Geçerli bir yorum seçiniz.");
    }
}
