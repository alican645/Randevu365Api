using FluentValidation;

namespace Randevu365.Application.Features.Register;

public class RegisterCustomerValidator : AbstractValidator<RegisterCustomerRequest>
{
    public RegisterCustomerValidator()
    {
        Include(new RegisterRequestValidator());
    }
}

public class RegisterBusinessOwnerValidator : AbstractValidator<RegisterBusinessOwnerRequest>
{
    public RegisterBusinessOwnerValidator()
    {
        Include(new RegisterRequestValidator());
    }
}

internal class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email adresi gereklidir.")
            .EmailAddress().WithMessage("Gecerli bir email adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Sifre gereklidir.")
            .MinimumLength(8).WithMessage("Sifre en az 8 karakter olmalidir.")
            .Matches(@"[A-Z]").WithMessage("Sifre en az bir buyuk harf icermelidir.")
            .Matches(@"[a-z]").WithMessage("Sifre en az bir kucuk harf icermelidir.")
            .Matches(@"\d").WithMessage("Sifre en az bir rakam icermelidir.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad gereklidir.")
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olmalidir.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Soyad gereklidir.")
            .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olmalidir.");



        RuleFor(x => x.VerificationCode)
            .NotEmpty().WithMessage("Doğrulama kodu gereklidir.")
            .Length(6).WithMessage("Doğrulama kodu 6 haneli olmalıdır.")
            .Matches(@"^\d{6}$").WithMessage("Doğrulama kodu sadece rakamlardan oluşmalıdır.");
    }
}
