using FluentValidation;

namespace Randevu365.Application.Features.Businesses.Commands.UpdateBusinessDetail;

public class UpdateBusinessDetailCommandValidator : AbstractValidator<UpdateBusinessDetailCommandRequest>
{
    public UpdateBusinessDetailCommandValidator()
    {
        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("İşletme adı boş olamaz.")
            .MaximumLength(100).WithMessage("İşletme adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.BusinessAddress)
            .NotEmpty().WithMessage("Adres boş olamaz.")
            .MaximumLength(250).WithMessage("Adres en fazla 250 karakter olabilir.");

        RuleFor(x => x.BusinessCity)
            .NotEmpty().WithMessage("Şehir boş olamaz.")
            .MaximumLength(50).WithMessage("Şehir adı en fazla 50 karakter olabilir.");

        RuleFor(x => x.BusinessPhone)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Matches(@"^(\+90|0)?[0-9]{10}$").WithMessage("Geçerli bir telefon numarası giriniz. Örn: 05551234567");

        RuleFor(x => x.BusinessEmail)
            .NotEmpty().WithMessage("Email adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

        RuleFor(x => x.BusinessCountry)
            .NotEmpty().WithMessage("Ülke boş olamaz.")
            .MaximumLength(50).WithMessage("Ülke adı en fazla 50 karakter olabilir.");

        RuleForEach(x => x.BusinessHours).ChildRules(hour =>
        {
            hour.RuleFor(h => h.Day).NotEmpty().WithMessage("Gün boş olamaz.");
            hour.RuleFor(h => h.OpenTime).NotEmpty().WithMessage("Açılış saati boş olamaz.");
            hour.RuleFor(h => h.CloseTime).NotEmpty().WithMessage("Kapanış saati boş olamaz.");
        });


    }
}
