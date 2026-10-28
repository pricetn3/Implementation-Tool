namespace ClientImplementation.Models;

/// <summary>When the additional paper product price applies.</summary>
public enum AdditionalPriceMode
{
    /// <summary>"Per cert type (Recommended)". Listed first; new clients default to it.</summary>
    PerCertType,
    /// <summary>"Per cart". Stays for HBSO.</summary>
    PerCart
}

/// <summary>
/// A shipping package a cert type can use, e.g. Large Tube 18" × 3" × 3".
/// "No packaging required" is not a package: it's <see cref="CertTypeSetting.NoPackagingRequired"/>.
/// </summary>
public sealed record PackagingOption(string Id, string Name, decimal Length, decimal Width, decimal Height)
{
    /// <summary>"Large Tube (18" L × 3" W × 3" H)".</summary>
    public string Label => $"{Name} ({Length:0.##}\" L × {Width:0.##}\" W × {Height:0.##}\" H)";
}

/// <summary>Paper description and packaging for one cert type.</summary>
public sealed record CertTypeSetting
{
    /// <summary>Stored packaging value for an explicit "No packaging required" (CEDs, microcredentials). Not the same as not selected.</summary>
    public const string NoPackagingRequired = "none";

    public const int DescriptionMaxLength = 300;

    /// <summary>Cert type code, e.g. 8511-LONGFORM-1SIG. Also the row key.</summary>
    public required string CertType { get; init; }

    /// <summary>What students see. Every cert type needs its own copy; "" when missing.</summary>
    public string Description { get; init; } = "";

    /// <summary>null = not selected ("Select Packaging"); <see cref="NoPackagingRequired"/>; or a <see cref="PackagingOption.Id"/>.</summary>
    public string? PackagingId { get; init; }

    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
    public bool HasPackaging => PackagingId is not null;
}

/// <summary>
/// Student Store paper ordering for one institution.
/// PaperOrderingEnabled is an organization-level flag: off keeps every saved value, on restores them.
/// Don't infer it from paper-product records.
/// </summary>
public sealed record StudentStoreSettings
{
    public bool PaperOrderingEnabled { get; init; }
    public decimal? FirstPrice { get; init; }
    public decimal? AdditionalPrice { get; init; }
    public AdditionalPriceMode PriceMode { get; init; } = AdditionalPriceMode.PerCertType;

    /// <summary>One row per cert type the institution has, in the institution's order.</summary>
    public IReadOnlyList<CertTypeSetting> CertTypes { get; init; } = [];

    public CertTypeSetting? Find(string certType) => CertTypes.FirstOrDefault(c => c.CertType == certType);

    /// <summary>Copy with the given cert types changed.</summary>
    public StudentStoreSettings WithCertTypes(IEnumerable<string> certTypes, Func<CertTypeSetting, CertTypeSetting> change)
    {
        var set = certTypes.ToHashSet();
        return this with { CertTypes = CertTypes.Select(c => set.Contains(c.CertType) ? change(c) : c).ToList() };
    }

    /// <summary>
    /// Nothing set up yet: no first price and no descriptions. Drives the note shown while paper ordering is off.
    /// </summary>
    public bool IsNotSetUp => FirstPrice is null && CertTypes.All(c => !c.HasDescription);

    // Value equality including the cert type list (the synthesized record Equals compares the list by reference).
    public bool Equals(StudentStoreSettings? other) =>
        other is not null
        && PaperOrderingEnabled == other.PaperOrderingEnabled
        && FirstPrice == other.FirstPrice
        && AdditionalPrice == other.AdditionalPrice
        && PriceMode == other.PriceMode
        && CertTypes.SequenceEqual(other.CertTypes);

    public override int GetHashCode() => HashCode.Combine(PaperOrderingEnabled, FirstPrice, AdditionalPrice, PriceMode, CertTypes.Count);
}
