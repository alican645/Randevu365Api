using FluentValidation;

namespace Randevu365.Application.Features.BusinessHours.Commands.DeleteBusinessHour;

public class DeleteBusinessHourCommandValidator : AbstractValidator<DeleteBusinessHourCommandRequest>
{
    public DeleteBusinessHourCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir çalışma saati seçiniz.");
    }
}
