namespace ClientImplementation.Models;

public sealed record Institution(string Id, string Name)
{
    /// <summary>Used in confirmation copy: "client 1111 (Virginia Hanock University)".</summary>
    public string Label => $"client {Id} ({Name})";
}

/// <summary>One row in the Actions Log. User is stored as text, so deleting a user doesn't change past entries.</summary>
public sealed class ActionsLogEntry
{
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public required string User { get; init; }
    public string? InstitutionId { get; init; }
    public string? InstitutionName { get; init; }
    /// <summary>Page or setting that changed, e.g. "API Order Batching".</summary>
    public required string Area { get; init; }
    public required string OldValue { get; init; }
    public required string NewValue { get; init; }
}

/// <summary>A validation error tied to the id of the field that gets focus on Submit.</summary>
public sealed record FieldError(string FieldId, string Message);
