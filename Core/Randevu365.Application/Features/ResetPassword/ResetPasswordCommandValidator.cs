using FluentValidation;

namespace Randevu365.Application.Features.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommandRequest>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email adresi gereklidir.")
            .EmailAddress().WithMessage("Gecerli bir email adresi giriniz.");

        RuleFor(x => x.ResetToken)
            .NotEmpty().WithMessage("Sifirlama kodu gereklidir.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni sifre gereklidir.")
            .MinimumLength(6).WithMessage("Sifre en az 6 karakter olmalidir.");
    }
}
