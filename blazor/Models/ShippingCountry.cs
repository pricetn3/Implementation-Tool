namespace ClientImplementation.Models;

/// <summary>
/// One country students can (or can't) ship paper orders to in the Student Store.
/// One list for every institution. Not used by Reorder.
/// Operations maintains it from UPS and USPS notices.
/// Future: address verification (type a few letters, pick a validated address).
/// </summary>
/// <param name="Code">ISO 3166-1 alpha-2 code, e.g. "US". Unique key.</param>
public sealed record ShippingCountry(string Code, string Name, bool Enabled);
