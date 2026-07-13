using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IEventLocationVoteRepository
{
    Task<EventLocationVote?> GetAsync(Guid optionId, Guid userId, CancellationToken cancellationToken);
    Task<List<EventLocationVote>> GetByOptionIdsAsync(IReadOnlyCollection<Guid> optionIds, CancellationToken cancellationToken);
    void Add(EventLocationVote vote);
    void Remove(EventLocationVote vote);
}
