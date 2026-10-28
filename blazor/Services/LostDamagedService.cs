using ClientImplementation.Models;

namespace ClientImplementation.Services;

public interface ILostDamagedService
{
    /// <returns>Saved settings; a new institution returns reporting off.</returns>
    Task<LostDamagedSettings> GetAsync(string institutionId);
    Task SaveAsync(string institutionId, LostDamagedSettings settings);
}
