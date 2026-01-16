using FluentValidation;

namespace Randevu365.Application.Features.Businesses.Commands.DeleteBusiness;

public class DeleteBusinessCommandValidator : AbstractValidator<DeleteBusinessCommandRequest>
{
    public DeleteBusinessCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("İşletme ID boş olamaz.");
    }

}