using FluentValidation;

namespace Randevu365.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandValidator : AbstractValidator<CancelAppointmentCommandRequest>
{
    public CancelAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .GreaterThan(0).WithMessage("Gecerli bir randevu ID'si giriniz.");

        RuleFor(x => x.CancellationReason)
            .MaximumLength(500).WithMessage("Iptal nedeni en fazla 500 karakter olabilir.")
            .When(x => x.CancellationReason != null);
    }
}
