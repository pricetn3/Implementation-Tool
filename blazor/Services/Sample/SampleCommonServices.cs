using ClientImplementation.Models;

namespace ClientImplementation.Services.Sample;

// In-memory stand-ins with the prototype's example data, so the pages run before the real
// data access exists. Replace each with a database-backed implementation.
// Register as Singleton so values survive page changes.

public sealed class SampleInstitutionService : IInstitutionService
{
    private static readonly Dictionary<string, Institution> All = new()
    {
        ["1111"] = new("1111", "Virginia Hanock University"),                  // set up
        ["2222"] = new("2222", "Virginia Hanock University Graduate School")   // new, nothing set up
    };

    public Task<Institution?> FindAsync(string id) => Task.FromResult(All.GetValueOrDefault(id.Trim()));
}

public sealed class SampleActionsLogService : IActionsLogService
{
    public List<ActionsLogEntry> Entries { get; } = [];

    public Task WriteAsync(ActionsLogEntry entry)
    {
        Entries.Add(entry);
        return Task.CompletedTask;
    }

    public Task WriteAsync(IEnumerable<ActionsLogEntry> entries)
    {
        Entries.AddRange(entries);
        return Task.CompletedTask;
    }
}

public sealed class SampleUsersService : IUsersService
{
    private readonly List<string> users = ["Brittany Krontz", "Katie Ameigh", "Quinton Spivey", "Theresia Dickie", "Tim Hord"];

    public Task<IReadOnlyList<string>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<string>>(users.Order(StringComparer.OrdinalIgnoreCase).ToList());

    public Task SaveAsync(IReadOnlyCollection<string> added, IReadOnlyCollection<string> deleted)
    {
        users.RemoveAll(deleted.Contains);
        users.AddRange(added);
        return Task.CompletedTask;
    }
}

public sealed class SampleLostDamagedService : ILostDamagedService
{
    private readonly Dictionary<string, LostDamagedSettings> saved = new()
    {
        ["1111"] = new() { ReportingEnabled = true, ApproveDamaged = true }
    };

    public Task<LostDamagedSettings> GetAsync(string institutionId) =>
        Task.FromResult(saved.GetValueOrDefault(institutionId) ?? new LostDamagedSettings());

    public Task SaveAsync(string institutionId, LostDamagedSettings settings)
    {
        saved[institutionId] = settings;
        return Task.CompletedTask;
    }
}
