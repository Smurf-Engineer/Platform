using Microsoft.EntityFrameworkCore;
using PlatformPlatform.AccountManagement.Domain.Tenants;
using PlatformPlatform.AccountManagement.Domain.Users;
using PlatformPlatform.SharedKernel.InfrastructureCore.EntityFramework;

namespace PlatformPlatform.AccountManagement.Infrastructure;

public sealed class AccountManagementDbContext : SharedKernelDbContext<AccountManagementDbContext>
{
    public AccountManagementDbContext(DbContextOptions<AccountManagementDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensures the strongly typed IDs can be saved and read by entity framework as the underlying type.
        modelBuilder.MapStronglyTypedId<Tenant, TenantId>(t => t.Id);
        modelBuilder.MapStronglyTypedId<User, UserId>(u => u.Id);
        modelBuilder.MapStronglyTypedId<User, TenantId>(u => u.TenantId);
    }
}