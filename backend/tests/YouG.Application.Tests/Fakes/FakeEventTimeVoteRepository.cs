using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeEventTimeVoteRepository : IEventTimeVoteRepository
{
    public List<EventTimeVote> Votes { get; } = [];

    public Task<EventTimeVote?> GetAsync(Guid optionId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(Votes.FirstOrDefault(v => v.EventTimeOptionId == optionId && v.UserId == userId));

    public Task<List<EventTimeVote>> GetByOptionIdsAsync(IReadOnlyCollection<Guid> optionIds, CancellationToken cancellationToken) =>
        Task.FromResult(Votes.Where(v => optionIds.Contains(v.EventTimeOptionId)).ToList());

    public void Add(EventTimeVote vote) => Votes.Add(vote);

    public void Remove(EventTimeVote vote) => Votes.Remove(vote);
}
