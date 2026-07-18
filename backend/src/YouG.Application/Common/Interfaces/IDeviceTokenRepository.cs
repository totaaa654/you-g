using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IDeviceTokenRepository
{
    Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);

    Task<List<DeviceToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    void Add(DeviceToken deviceToken);

    void Remove(DeviceToken deviceToken);

    void RemoveRange(IEnumerable<DeviceToken> deviceTokens);
}
