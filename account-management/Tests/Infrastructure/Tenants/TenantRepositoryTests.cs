using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformPlatform.AccountManagement.Application;
using PlatformPlatform.AccountManagement.Domain.Tenants;
using PlatformPlatform.AccountManagement.Infrastructure;
using PlatformPlatform.AccountManagement.Infrastructure.Tenants;
using PlatformPlatform.SharedKernel.DomainModeling;
using Xunit;

namespace PlatformPlatform.AccountManagement.Tests.Infrastructure.Tenants;

public sealed class TenantRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly SqliteInMemoryDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly TenantRepository _tenantRepository;

    public TenantRepositoryTests()
    {
        var services = new ServiceCollection();

        _dbContextFactory = new SqliteInMemoryDbContextFactory<ApplicationDbContext>();
        _applicationDbContext = _dbContextFactory.CreateContext();
        services.AddDomainModelingServices(ApplicationConfiguration.Assembly);

        _tenantRepository = new TenantRepository(_applicationDbContext);
    }

    public void Dispose()
    {
        _dbContextFactory.Dispose();
    }

    [Fact]
    public async Task Add_WhenTenantDoesNotExist_ShouldAddTenantToDatabase()
    {
        // Arrange
        var tenant = Tenant.Create("New Tenant", "new", "new@test.com", "1234567890");

        // Act
        await _tenantRepository.AddAsync(tenant);
        await _applicationDbContext.SaveChangesAsync();

        // Assert
        var retrievedTenant = await _applicationDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenant.Id);
        retrievedTenant.Should().NotBeNull();
        retrievedTenant!.Id.Should().Be(tenant.Id);
    }

    [Fact]
    public async Task Update_WhenTenantExists_ShouldUpdateTenantInDatabase()
    {
        // Arrange
        var tenant = Tenant.Create("Existing Tenant", "existing", "existing@test.com", "1234567890");
        await _applicationDbContext.Tenants.AddAsync(tenant);
        await _applicationDbContext.SaveChangesAsync();

        // Act
        tenant.Update("Updated Tenant", "existing@test.com", "1234567890");
        _tenantRepository.Update(tenant);
        await _applicationDbContext.SaveChangesAsync();

        // Assert
        var updatedTenant = await _applicationDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenant.Id);
        updatedTenant.Should().NotBeNull();
        updatedTenant!.Name.Should().Be("Updated Tenant");
    }

    [Fact]
    public async Task Remove_WhenTenantExists_ShouldRemoveTenantFromDatabase()
    {
        // Arrange
        var tenant = Tenant.Create("Existing Tenant", "existing", "existing@test.com", "1234567890");
        await _applicationDbContext.Tenants.AddAsync(tenant);
        await _applicationDbContext.SaveChangesAsync();

        // Act
        _tenantRepository.Remove(tenant);
        await _applicationDbContext.SaveChangesAsync();

        // Assert
        var removedTenant = await _applicationDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenant.Id);
        removedTenant.Should().BeNull();
    }

    [Fact]
    public async Task IsSubdomainFreeAsync_WhenSubdomainAlreadyExists_ShouldReturnFalse()
    {
        // Arrange  
        var tenant = Tenant.Create("Existing Tenant", "existing", "existing@test.com", "1234567890");

        await _applicationDbContext.Tenants.AddAsync(tenant);
        await _applicationDbContext.SaveChangesAsync();

        // Act
        var isSubdomainFree = await _tenantRepository.IsSubdomainFreeAsync("existing", CancellationToken.None);

        // Assert
        isSubdomainFree.Should().BeFalse();
    }

    [Fact]
    public async Task IsSubdomainFreeAsync_WhenSubdomainDoesNotExist_ShouldReturnTrue()
    {
        // Arrange
        var tenant = Tenant.Create("Existing Tenant", "existing", "existing@test.com", "1234567890");

        await _applicationDbContext.Tenants.AddAsync(tenant);
        await _applicationDbContext.SaveChangesAsync();

        // Act
        var isSubdomainFree = await _tenantRepository.IsSubdomainFreeAsync("nonexistent", CancellationToken.None);

        // Assert
        isSubdomainFree.Should().BeTrue();
    }
}