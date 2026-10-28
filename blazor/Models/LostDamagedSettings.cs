namespace ClientImplementation.Models;

/// <summary>
/// Lost &amp; Damaged reporting for one institution.
/// February launch shows only ReportingEnabled. Requests are auto-approved into a replacement order.
/// ApproveLost / ApproveDamaged are for the later approval release; keep them in the data model.
/// ReportingEnabled is its own organization-level flag, separate from Student Store paper ordering.
/// </summary>
public sealed record LostDamagedSettings
{
    public bool ReportingEnabled { get; init; }
    public bool ApproveLost { get; init; }
    public bool ApproveDamaged { get; init; }
}
