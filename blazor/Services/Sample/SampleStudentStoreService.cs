using System.Text.RegularExpressions;
using ClientImplementation.Models;

namespace ClientImplementation.Services.Sample;

// In-memory stand-in with the prototype's example data. Register as Singleton.
// 1111: set up (HBSO's 11 cert types, paper ordering on, Per cart).
// 2222: new, nothing set up (graduate cert types, paper ordering off, Per cert type).
public sealed class SampleStudentStoreService : IStudentStoreService
{
    private static readonly IReadOnlyList<PackagingOption> Packaging =
    [
        new("large-tube", "Large Tube", 18, 3, 3),
        new("small-tube", "Small Tube", 15, 2, 2),
        new("mailer-5", "#5 Mailer", 13, 10, 1),
        new("mailer-4", "#4 Mailer", 15, 13, 1),
        new("mailer-6", "#6 Mailer", 18, 13, 1),
        new("mailer-7", "#7 Mailer", 21, 17, 1)
    ];

    // The prototype also has a "Typical client" set (8511-BA, 8511-BS, 8511-MA, 8511-PHD, MICROCREDENTIAL)
    // and a 28-row "Large client" set for testing the grid.
    private static readonly string[] Hbso =
    [
        "8511-CLIMB", "8511-CSMLLONGFORM", "8511-CSMLSPEC", "8511-LONGFORM-1SIG", "8511-LONGFORM-2SIG", "8511-LONGFORM-3SIG",
        "8511-SPEC-CORE", "8511-SPEC-CRED-1SIG", "8511-SPEC-CRED-3SIG", "8511-SPEC", "MICROCREDENTIAL"
    ];

    private static readonly string[] Grad =
        ["8511-MA", "8511-MS", "8511-MBA", "8511-MED", "8511-EDD", "8511-PHD", "8511-CERT-GRAD", "MICROCREDENTIAL"];

    private const string DescLong = "Your official long-form diploma, printed on heavyweight parchment with the university seal. It ships rolled in a rigid tube so it arrives without creases.";
    private const string DescSpec = "Your official specialization certificate, printed on secure paper and shipped flat in a protective mailer.";
    private const string DescCsml = "Your official certificate of specialized management, printed on secure paper and shipped flat in a protective mailer.";
    private const string DescDegree = "Your official diploma, printed on secure paper with the university seal and shipped in protective packaging.";
    private const string DescClimb = "Your official CLIMB certificate, printed on secure paper and shipped flat in a protective mailer.";
    private const string DescMicro = "A printed copy of your microcredential on standard paper. Ships in a plain envelope.";

    private readonly Dictionary<string, StudentStoreSettings> saved = new()
    {
        ["1111"] = new()
        {
            PaperOrderingEnabled = true,
            FirstPrice = 25.00m,
            AdditionalPrice = 15.00m,
            PriceMode = AdditionalPriceMode.PerCart,
            CertTypes = Hbso.Select(SetUp).ToList()
        },
        ["2222"] = new()
        {
            PaperOrderingEnabled = false,
            PriceMode = AdditionalPriceMode.PerCertType,
            CertTypes = Grad.Select(n => new CertTypeSetting { CertType = n }).ToList()
        }
    };

    public Task<StudentStoreSettings> GetAsync(string institutionId) =>
        Task.FromResult(saved.GetValueOrDefault(institutionId) ?? new StudentStoreSettings());

    public Task<IReadOnlyList<PackagingOption>> GetPackagingOptionsAsync() => Task.FromResult(Packaging);

    public Task SaveAsync(string institutionId, StudentStoreSettings settings)
    {
        saved[institutionId] = settings;
        return Task.CompletedTask;
    }

    // Same rules the prototype uses to fill a set-up client.
    private static CertTypeSetting SetUp(string n) => new()
    {
        CertType = n,
        PackagingId =
            n.Contains("MICRO") ? CertTypeSetting.NoPackagingRequired
            : Regex.IsMatch(n, "LONGFORM|PHD|EDD|JD|MBA|MED|MA$|MS$") ? "large-tube"
            : n.Contains("CSML") ? "mailer-6"
            : "mailer-5",
        Description =
            n.Contains("MICRO") ? DescMicro
            : n.Contains("LONGFORM") ? DescLong
            : n.Contains("CSML") ? DescCsml
            : n.Contains("SPEC") ? DescSpec
            : n.Contains("CLIMB") ? DescClimb
            : DescDegree
    };
}
