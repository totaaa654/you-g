using FluentValidation;

namespace YouG.Application.Features.Availability.Queries.GetMyAvailabilityInstances;

public class GetMyAvailabilityInstancesQueryValidator : AbstractValidator<GetMyAvailabilityInstancesQuery>
{
    public GetMyAvailabilityInstancesQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
        RuleFor(x => x).Must(x => x.To.DayNumber - x.From.DayNumber <= 90)
            .WithMessage("Date range cannot exceed 90 days.");
    }
}
