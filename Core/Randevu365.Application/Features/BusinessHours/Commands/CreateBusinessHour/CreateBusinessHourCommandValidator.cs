using FluentValidation;

namespace Randevu365.Application.Features.BusinessHours.Commands.CreateBusinessHour;

public class CreateBusinessHourCommandValidator : AbstractValidator<CreateBusinessHourCommandRequest>
{
    public CreateBusinessHourCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty().WithMessage("İşyeri Id değeri boş olamaz.");
        
    }
}