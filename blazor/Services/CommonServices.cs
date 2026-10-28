using ClientImplementation.Models;

namespace ClientImplementation.Services;

/// <summary>Institution lookup. Wire to the same source the Ced/CeC search uses.</summary>
public interface IInstitutionService
{
    /// <returns>null when no institution has that ID.</returns>
    Task<Institution?> FindAsync(string id);
}

/// <summary>Writes to the existing Actions Log.</summary>
public interface IActionsLogService
{
    Task WriteAsync(ActionsLogEntry entry);
    Task WriteAsync(IEnumerable<ActionsLogEntry> entries);
}

/// <summary>Names shown in the User dropdown on every page. Maintained on the Users page.</summary>
public interface IUsersService
{
    /// <returns>Names, A–Z.</returns>
    Task<IReadOnlyList<string>> GetAllAsync();
    Task SaveAsync(IReadOnlyCollection<string> added, IReadOnlyCollection<string> deleted);
}

/// <summary>
/// Who is making changes, picked in the User dropdown. Register as Scoped so the choice
/// stays selected across pages for the session (Blazor Server: per circuit).
/// To keep it across reloads too, persist UserName with ProtectedLocalStorage.
/// If Windows authentication becomes possible, replace this with the signed-in user.
/// </summary>
public sealed class CurrentUserState
{
    public string? UserName { get; private set; }

    /// <summary>True after a Submit was blocked because no user was selected.</summary>
    public bool ShowRequiredError { get; set; }

    /// <summary>Raised when the selection changes or the Users list is saved.</summary>
    public event Action? Changed;

    public bool HasUser => !string.IsNullOrEmpty(UserName);

    public void Select(string? userName)
    {
        UserName = string.IsNullOrEmpty(userName) ? null : userName;
        if (HasUser) ShowRequiredError = false;
        Changed?.Invoke();
    }

    /// <summary>Call after the Users page saves, so every User dropdown reloads. Resets if the selected user was deleted.</summary>
    public void UsersChanged(IReadOnlyCollection<string> currentNames)
    {
        if (UserName is not null && !currentNames.Contains(UserName)) UserName = null;
        Changed?.Invoke();
    }

    public void NotifyChanged() => Changed?.Invoke();
}
