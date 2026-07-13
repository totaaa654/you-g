using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class EventLocationVoteRepository(YouGDbContext dbContext) : IEventLocationVoteRepository
{
    public Task<EventLocationVote?> GetAsync(Guid optionId, Guid userId, CancellationToken cancellationToken) =>
        dbContext.EventLocationVotes.FirstOrDefaultAsync(v => v.EventLocationOptionId == optionId && v.UserId == userId, cancellationToken);

    public Task<List<EventLocationVote>> GetByOptionIdsAsync(IReadOnlyCollection<Guid> optionIds, CancellationToken cancellationToken) =>
        dbContext.EventLocationVotes.Where(v => optionIds.Contains(v.EventLocationOptionId)).ToListAsync(cancellationToken);

    public void Add(EventLocationVote vote) => dbContext.EventLocationVotes.Add(vote);

    public void Remove(EventLocationVote vote) => dbContext.EventLocationVotes.Remove(vote);
}
