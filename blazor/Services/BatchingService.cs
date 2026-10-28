using ClientImplementation.Models;

namespace ClientImplementation.Services;

public interface IBatchingService
{
    /// <returns>Saved standard-order schedule; a new institution returns an empty schedule ("Not set").</returns>
    Task<BatchingSchedule> GetAsync(string institutionId);

    /// <summary>Saves schedule.Normalized() (only the days the chosen frequency uses).</summary>
    Task SaveAsync(string institutionId, BatchingSchedule schedule);
}
