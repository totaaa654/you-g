using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class EventTimeVoteRepository(YouGDbContext dbContext) : IEventTimeVoteRepository
{
    public Task<EventTimeVote?> GetAsync(Guid optionId, Guid userId, CancellationToken cancellationToken) =>
        dbContext.EventTimeVotes.FirstOrDefaultAsync(v => v.EventTimeOptionId == optionId && v.UserId == userId, cancellationToken);

    public Task<List<EventTimeVote>> GetByOptionIdsAsync(IReadOnlyCollection<Guid> optionIds, CancellationToken cancellationToken) =>
        dbContext.EventTimeVotes.Where(v => optionIds.Contains(v.EventTimeOptionId)).ToListAsync(cancellationToken);

    public void Add(EventTimeVote vote) => dbContext.EventTimeVotes.Add(vote);

    public void Remove(EventTimeVote vote) => dbContext.EventTimeVotes.Remove(vote);
}
