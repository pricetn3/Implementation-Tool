using ClientImplementation.Models;

namespace ClientImplementation.Services.Sample;

// In-memory stand-in with the prototype's example data. Replace with a database-backed implementation.
// Register as Singleton so values survive page changes.
public sealed class SampleBatchingService : IBatchingService
{
    private readonly Dictionary<string, BatchingSchedule> saved = new()
    {
        // 1111 is set up: Weekly on Tuesday. 2222 is new: nothing saved, so "Not set".
        ["1111"] = new() { Frequency = BatchFrequency.Weekly, WeeklyDay = DayOfWeek.Tuesday }
    };

    public Task<BatchingSchedule> GetAsync(string institutionId) =>
        Task.FromResult(saved.GetValueOrDefault(institutionId) ?? new BatchingSchedule());

    public Task SaveAsync(string institutionId, BatchingSchedule schedule)
    {
        saved[institutionId] = schedule.Normalized();
        return Task.CompletedTask;
    }
}
