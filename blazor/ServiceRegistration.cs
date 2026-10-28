using ClientImplementation.Services;
using ClientImplementation.Services.Sample;

namespace ClientImplementation;

public static class NewTabsServiceRegistration
{
    /// <summary>
    /// In Program.cs, after builder.Services.AddTelerikBlazor():
    ///     builder.Services.AddNewTabs();
    /// Sample services hold the prototype's example data in memory. Swap each one for the
    /// real implementation as the data access is built (same interface, Scoped).
    /// </summary>
    public static IServiceCollection AddNewTabs(this IServiceCollection services)
    {
        // Per user session: who is making changes (User dropdown).
        services.AddScoped<CurrentUserState>();

        // Replace these with the real implementations.
        services.AddSingleton<IInstitutionService, SampleInstitutionService>();
        services.AddSingleton<IActionsLogService, SampleActionsLogService>();
        services.AddSingleton<IUsersService, SampleUsersService>();
        services.AddSingleton<IBatchingService, SampleBatchingService>();
        services.AddSingleton<IStudentStoreService, SampleStudentStoreService>();
        services.AddSingleton<ILostDamagedService, SampleLostDamagedService>();
        services.AddSingleton<ICountryService, SampleCountryService>();

        return services;
    }
}
