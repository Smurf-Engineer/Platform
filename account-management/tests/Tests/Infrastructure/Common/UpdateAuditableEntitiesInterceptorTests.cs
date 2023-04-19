using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PlatformPlatform.AccountManagement.Domain.Tenants;
using PlatformPlatform.AccountManagement.Infrastructure;
using Xunit;

namespace PlatformPlatform.AccountManagement.Tests.Infrastructure.Common;

public class UpdateAuditableEntitiesInterceptorTests
{
    [Fact]
    public async Task SavingChangesAsync_WhenEntityIsAdded_ShouldSetCreatedAt()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        await using var dbContext = new TestDbContext(options);
        var newTenant = new Tenant {Name = "TestTenant"};

        // Act
        dbContext.Tenants.Add(newTenant);
        await dbContext.SaveChangesAsync();

        // Assert
        newTenant.CreatedAt.Should().NotBe(default);
        newTenant.ModifiedAt.Should().BeNull();
    }

    [Fact]
    public async Task SavingChangesAsync_WhenEntityIsModified_ShouldUpdateModifiedAt()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        await using var dbContext = new TestDbContext(options);
        var newTenant = new Tenant {Name = "TestTenant"};
        dbContext.Tenants.Add(newTenant);
        await dbContext.SaveChangesAsync();
        var initialCreatedAt = newTenant.CreatedAt;
        var initialModifiedAt = newTenant.ModifiedAt;

        // Act
        newTenant.Name = "UpdatedTenant";
        await dbContext.SaveChangesAsync();

        // Assert
        newTenant.ModifiedAt.Should().NotBe(default);
        newTenant.ModifiedAt.Should().NotBe(initialModifiedAt);
        newTenant.CreatedAt.Should().Be(initialCreatedAt);
    }

    private class TestDbContext : ApplicationDbContext
    {
        public TestDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryDbForTesting");
            base.OnConfiguring(optionsBuilder);
        }
    }
}