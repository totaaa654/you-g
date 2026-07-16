using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeEventLocationVoteRepository : IEventLocationVoteRepository
{
    public List<EventLocationVote> Votes { get; } = [];

    public Task<EventLocationVote?> GetAsync(Guid optionId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(Votes.FirstOrDefault(v => v.EventLocationOptionId == optionId && v.UserId == userId));

    public Task<List<EventLocationVote>> GetByOptionIdsAsync(IReadOnlyCollection<Guid> optionIds, CancellationToken cancellationToken) =>
        Task.FromResult(Votes.Where(v => optionIds.Contains(v.EventLocationOptionId)).ToList());

    public void Add(EventLocationVote vote) => Votes.Add(vote);

    public void Remove(EventLocationVote vote) => Votes.Remove(vote);
}
