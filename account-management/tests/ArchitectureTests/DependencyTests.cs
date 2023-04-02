using FluentAssertions;
using NetArchTest.Rules;
using PlatformPlatform.Application;
using PlatformPlatform.Domain;
using PlatformPlatform.Infrastructure;
using PlatformPlatform.WebApi;

namespace PlatformPlatform.ArchitectureTests;

public class DependencyTests
{
    [Fact]
    public void WebApi_ShouldNot_HaveDependencyOnInfrastructure()
    {
        // Act
        var result = Types
            .InAssembly(WebApiAssembly.Assembly)
            .That()
            .DoNotHaveNameMatching("Program") // The Program class are allowed to register infrastructure services
            .ShouldNot()
            .HaveDependencyOn(InfrastructureAssembly.Name)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNot_HaveDependencyOnInfrastructureAndWebApi()
    {
        // Arrange
        string[] otherAssemblies =
        {
            InfrastructureAssembly.Name,
            WebApiAssembly.Name
        };

        // Act
        var result = Types
            .InAssembly(ApplicationAssembly.Assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherAssemblies)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ShouldNot_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var otherAssemblies = new[]
        {
            ApplicationAssembly.Name,
            InfrastructureAssembly.Name,
            WebApiAssembly.Name
        };

        // Act
        var result = Types
            .InAssembly(DomainAssembly.Assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherAssemblies)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}