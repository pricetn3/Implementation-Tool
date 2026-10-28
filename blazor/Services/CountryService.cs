using ClientImplementation.Models;

namespace ClientImplementation.Services;

/// <summary>Shipping countries for the Student Store. One global list, not per institution.</summary>
public interface ICountryService
{
    /// <returns>Every country, A–Z by name.</returns>
    Task<IReadOnlyList<ShippingCountry>> GetAllAsync();

    /// <summary>Saves only the countries whose Enabled flag changed.</summary>
    Task SaveAsync(IReadOnlyCollection<ShippingCountry> changed);
}
