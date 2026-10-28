using ClientImplementation.Models;

namespace ClientImplementation.Services.Sample;

// In-memory stand-in with the prototype's country list. Replace with a database-backed implementation.
// Register as Singleton so values survive page changes.
public sealed class SampleCountryService : ICountryService
{
    private readonly List<ShippingCountry> countries =
    [
        new("AQ", "Antarctica", false),
        new("AR", "Argentina", true),
        new("AU", "Australia", true),
        new("AT", "Austria", true),
        new("BS", "Bahamas", true),
        new("BD", "Bangladesh", true),
        new("BE", "Belgium", true),
        new("BM", "Bermuda", true),
        new("BV", "Bouvet Island", false),
        new("BR", "Brazil", true),
        new("CA", "Canada", true),
        new("CL", "Chile", true),
        new("CN", "China", true),
        new("CO", "Colombia", true),
        new("CR", "Costa Rica", true),
        new("CZ", "Czechia", true),
        new("DK", "Denmark", true),
        new("DO", "Dominican Republic", true),
        new("EG", "Egypt", true),
        new("FI", "Finland", true),
        new("FR", "France", true),
        new("DE", "Germany", true),
        new("GH", "Ghana", true),
        new("GR", "Greece", true),
        new("HM", "Heard Island and McDonald Islands", false),
        new("HK", "Hong Kong", true),
        new("IS", "Iceland", true),
        new("IN", "India", true),
        new("ID", "Indonesia", true),
        new("IE", "Ireland", true),
        new("IL", "Israel", true),
        new("IT", "Italy", true),
        new("JM", "Jamaica", true),
        new("JP", "Japan", true),
        new("KE", "Kenya", true),
        new("MY", "Malaysia", true),
        new("MX", "Mexico", true),
        new("NL", "Netherlands", true),
        new("NZ", "New Zealand", true),
        new("NG", "Nigeria", true),
        new("NO", "Norway", true),
        new("PK", "Pakistan", true),
        new("PE", "Peru", true),
        new("PH", "Philippines", true),
        new("PL", "Poland", true),
        new("PT", "Portugal", true),
        new("PR", "Puerto Rico", true),
        new("SA", "Saudi Arabia", true),
        new("SG", "Singapore", true),
        new("ZA", "South Africa", true),
        new("GS", "South Georgia and the South Sandwich Islands", false),
        new("KR", "South Korea", true),
        new("ES", "Spain", true),
        new("SE", "Sweden", true),
        new("CH", "Switzerland", true),
        new("TW", "Taiwan", true),
        new("TH", "Thailand", true),
        new("TR", "Türkiye", true),
        new("AE", "United Arab Emirates", true),
        new("GB", "United Kingdom", true),
        new("US", "United States", true),
        new("VN", "Vietnam", true)
    ];

    public Task<IReadOnlyList<ShippingCountry>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<ShippingCountry>>(countries.OrderBy(c => c.Name, StringComparer.CurrentCultureIgnoreCase).ToList());

    public Task SaveAsync(IReadOnlyCollection<ShippingCountry> changed)
    {
        foreach (var c in changed)
        {
            var i = countries.FindIndex(x => x.Code == c.Code);
            if (i >= 0) countries[i] = c;
        }
        return Task.CompletedTask;
    }
}
