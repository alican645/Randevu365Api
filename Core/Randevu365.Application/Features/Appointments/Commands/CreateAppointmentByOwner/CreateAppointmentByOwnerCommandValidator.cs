using FluentValidation;

namespace Randevu365.Application.Features.Appointments.Commands.CreateAppointmentByOwner;

public class CreateAppointmentByOwnerCommandValidator : AbstractValidator<CreateAppointmentByOwnerCommandRequest>
{
    public CreateAppointmentByOwnerCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme seçiniz.");

        RuleFor(x => x.BusinessServiceId)
            .GreaterThan(0).WithMessage("Geçerli bir hizmet seçiniz.");

        RuleFor(x => x.AppUserId)
            .GreaterThan(0).WithMessage("Geçerli bir müşteri seçiniz.");

        RuleFor(x => x.AppointmentDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Randevu tarihi geçmiş bir tarih olamaz.");

        RuleFor(x => x.RequestedStartTime)
            .LessThan(x => x.RequestedEndTime).WithMessage("Başlangıç saati bitiş saatinden önce olmalıdır.");

        RuleFor(x => x.CustomerNotes)
            .MaximumLength(500).WithMessage("Müşteri notları en fazla 500 karakter olabilir.")
            .When(x => x.CustomerNotes != null);

        RuleFor(x => x.BusinessNotes)
            .MaximumLength(500).WithMessage("İşletme notları en fazla 500 karakter olabilir.")
            .When(x => x.BusinessNotes != null);
    }
}
