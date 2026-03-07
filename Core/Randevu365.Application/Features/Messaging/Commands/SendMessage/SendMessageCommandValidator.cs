using FluentValidation;

namespace Randevu365.Application.Features.Messaging.Commands.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommandRequest>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ReceiverId)
            .GreaterThan(0).WithMessage("Gecerli bir alici ID'si giriniz.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Mesaj icerigi bos olamaz.")
            .MaximumLength(2000).WithMessage("Mesaj en fazla 2000 karakter olabilir.");
    }
}
