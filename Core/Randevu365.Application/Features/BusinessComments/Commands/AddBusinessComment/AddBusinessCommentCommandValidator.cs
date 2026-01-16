using FluentValidation;

namespace Randevu365.Application.Features.BusinessComments.Commands.AddBusinessComment;

public class AddBusinessCommentCommandValidator : AbstractValidator<AddBusinessCommentCommandRequest>
{
    public AddBusinessCommentCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme seçiniz.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Yorum boş olamaz.")
            .MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olabilir.");
    }
}
