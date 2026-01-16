using FluentValidation;

namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentByCommentId;

public class GetCommentByCommentIdQueryValidator : AbstractValidator<GetCommentByCommentIdQueryRequest>
{
    public GetCommentByCommentIdQueryValidator()
    {
        RuleFor(x => x.CommentId)
            .GreaterThan(0).WithMessage("Geçerli bir yorum ID'si giriniz.");
    }
}
