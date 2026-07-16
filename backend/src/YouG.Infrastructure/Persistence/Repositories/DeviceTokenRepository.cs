using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class DeviceTokenRepository(YouGDbContext dbContext) : IDeviceTokenRepository
{
    public Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken cancellationToken) =>
        dbContext.DeviceTokens.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);

    public Task<List<DeviceToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.DeviceTokens.Where(t => t.UserId == userId).ToListAsync(cancellationToken);

    public void Add(DeviceToken deviceToken) => dbContext.DeviceTokens.Add(deviceToken);

    public void Remove(DeviceToken deviceToken) => dbContext.DeviceTokens.Remove(deviceToken);

    public void RemoveRange(IEnumerable<DeviceToken> deviceTokens) => dbContext.DeviceTokens.RemoveRange(deviceTokens);
}
