using ClientImplementation.Models;

namespace ClientImplementation.Services;

public interface IStudentStoreService
{
    /// <returns>
    /// Saved settings with one row per cert type the institution has.
    /// A new institution returns paper ordering off, no prices, Per cert type, and every row with no description and packaging not selected.
    /// </returns>
    Task<StudentStoreSettings> GetAsync(string institutionId);

    /// <returns>Packages a cert type can use, in menu order. "No packaging required" is not in this list.</returns>
    Task<IReadOnlyList<PackagingOption>> GetPackagingOptionsAsync();

    Task SaveAsync(string institutionId, StudentStoreSettings settings);
}
