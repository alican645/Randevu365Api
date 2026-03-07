using FluentValidation;

namespace Randevu365.Application.Features.UserProfile.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommandRequest>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Mevcut sifre gereklidir.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni sifre gereklidir.")
            .MinimumLength(6).WithMessage("Yeni sifre en az 6 karakter olmalidir.")
            .NotEqual(x => x.CurrentPassword).WithMessage("Yeni sifre mevcut sifreden farkli olmalidir.");
    }
}
