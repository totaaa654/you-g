using FluentValidation;

namespace YouG.Application.Features.Profile.Queries.SearchUsers;

public class SearchUsersQueryValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersQueryValidator()
    {
        RuleFor(x => x.Query).NotEmpty().MinimumLength(1).MaximumLength(32);
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
