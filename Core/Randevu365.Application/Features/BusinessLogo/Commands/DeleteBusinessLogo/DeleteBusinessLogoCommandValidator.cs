using FluentValidation;

namespace Randevu365.Application.Features.BusinessLogo.Commands.DeleteBusinessLogo;

public class DeleteBusinessLogoCommandValidator : AbstractValidator<DeleteBusinessLogoCommandRequest>
{
    public DeleteBusinessLogoCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme seçiniz.");
    }
}
