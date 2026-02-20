using FluentValidation;

namespace Randevu365.Application.Features.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommandRequest>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme seçiniz.");

        RuleFor(x => x.BusinessServiceId)
            .GreaterThan(0).WithMessage("Geçerli bir hizmet seçiniz.");

        RuleFor(x => x.AppointmentDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Randevu tarihi geçmiş bir tarih olamaz.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime).WithMessage("Başlangıç saati bitiş saatinden önce olmalıdır.");

        RuleFor(x => x.CustomerNotes)
            .MaximumLength(500).WithMessage("Müşteri notları en fazla 500 karakter olabilir.")
            .When(x => x.CustomerNotes != null);
    }
}
