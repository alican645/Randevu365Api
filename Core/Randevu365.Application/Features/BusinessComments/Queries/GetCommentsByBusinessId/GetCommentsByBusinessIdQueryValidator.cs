using FluentValidation;

namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentsByBusinessId;

public class GetCommentsByBusinessIdQueryValidator : AbstractValidator<GetCommentsByBusinessIdQueryRequest>
{
    public GetCommentsByBusinessIdQueryValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme ID'si giriniz.");
    }
}
