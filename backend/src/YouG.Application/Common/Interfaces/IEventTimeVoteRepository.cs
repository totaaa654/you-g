using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IEventTimeVoteRepository
{
    Task<EventTimeVote?> GetAsync(Guid optionId, Guid userId, CancellationToken cancellationToken);
    Task<List<EventTimeVote>> GetByOptionIdsAsync(IReadOnlyCollection<Guid> optionIds, CancellationToken cancellationToken);
    void Add(EventTimeVote vote);
    void Remove(EventTimeVote vote);
}
