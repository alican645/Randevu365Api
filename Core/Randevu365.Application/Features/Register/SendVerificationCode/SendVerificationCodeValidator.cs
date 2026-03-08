using FluentValidation;

namespace Randevu365.Application.Features.Register.SendVerificationCode;

public class SendVerificationCodeValidator : AbstractValidator<SendVerificationCodeRequest>
{
    public SendVerificationCodeValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email adresi gereklidir.")
            .EmailAddress().WithMessage("Gecerli bir email adresi giriniz.");
    }
}
