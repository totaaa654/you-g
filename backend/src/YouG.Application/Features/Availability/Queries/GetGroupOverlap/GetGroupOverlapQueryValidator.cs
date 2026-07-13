using FluentValidation;

namespace YouG.Application.Features.Availability.Queries.GetGroupOverlap;

public class GetGroupOverlapQueryValidator : AbstractValidator<GetGroupOverlapQuery>
{
    public GetGroupOverlapQueryValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
        RuleFor(x => x).Must(x => x.To.DayNumber - x.From.DayNumber <= 60)
            .WithMessage("Date range cannot exceed 60 days.");
    }
}
