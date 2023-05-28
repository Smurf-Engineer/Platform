using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PlatformPlatform.SharedKernel.DomainModeling;

namespace PlatformPlatform.AccountManagement.Application;

/// <summary>
///     The ApplicationConfiguration class is used to register services used by the application layer
///     with the dependency injection container.
/// </summary>
public static class ApplicationConfiguration
{
    public static Assembly Assembly => Assembly.GetExecutingAssembly();

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddDomainModelingServices(Assembly);

        return services;
    }
}