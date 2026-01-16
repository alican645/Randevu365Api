using FluentValidation;

namespace Randevu365.Application.Features.BusinessHours.Commands.UpdateBusinessHour;

public class UpdateBusinessHourCommandValidator : AbstractValidator<UpdateBusinessHourCommandRequest>
{
    public UpdateBusinessHourCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir çalışma saati seçiniz.");

        RuleFor(x => x.Day)
            .NotEmpty().WithMessage("Gün boş olamaz.")
            .Must(BeValidDay).WithMessage("Geçerli bir gün giriniz (Pazartesi, Salı, vb.).");

        RuleFor(x => x.OpenTime)
            .NotEmpty().WithMessage("Açılış saati boş olamaz.")
            .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$").WithMessage("Açılış saati formatı geçersiz. Örn: 09:00");

        RuleFor(x => x.CloseTime)
            .NotEmpty().WithMessage("Kapanış saati boş olamaz.")
            .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$").WithMessage("Kapanış saati formatı geçersiz. Örn: 18:00");
    }

    private bool BeValidDay(string day)
    {
        var validDays = new[] { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi", "Pazar",
                                 "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        return validDays.Contains(day, StringComparer.OrdinalIgnoreCase);
    }
}
