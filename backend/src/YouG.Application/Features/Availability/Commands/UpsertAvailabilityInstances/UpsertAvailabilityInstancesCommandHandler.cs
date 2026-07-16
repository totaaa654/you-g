using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Availability.Commands.UpsertAvailabilityInstances;

public class UpsertAvailabilityInstancesCommandHandler(
    IAvailabilityInstanceRepository instanceRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<UpsertAvailabilityInstancesCommand>
{
    public async Task Handle(UpsertAvailabilityInstancesCommand request, CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;

        foreach (var upsert in request.Instances)
        {
            var existing = await instanceRepository.GetAsync(currentUser.UserId, upsert.Date, upsert.StartTime, cancellationToken);

            if (existing is null)
            {
                instanceRepository.Add(new AvailabilityInstance
                {
                    UserId = currentUser.UserId,
                    Date = upsert.Date,
                    StartTime = upsert.StartTime,
                    Status = upsert.Status,
                    SourceRuleId = null, // manual override
                    UpdatedAt = now
                });
            }
            else
            {
                // A manual edit always overrides whatever the recurrence sweep generated,
                // per docs/03-DATABASE.md Section 3.9.
                existing.Status = upsert.Status;
                existing.SourceRuleId = null;
                existing.UpdatedAt = now;
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
