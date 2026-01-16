using FluentValidation;

namespace Randevu365.Application.Features.BusinessComments.Commands.UpdateBusinessComment;

public class UpdateBusinessCommentCommandValidator : AbstractValidator<UpdateBusinessCommentCommandRequest>
{
    public UpdateBusinessCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .GreaterThan(0).WithMessage("Geçerli bir yorum seçiniz.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Yorum boş olamaz.")
            .MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olabilir.");
    }
}
