using PlatformPlatform.AccountManagement.Domain.Primitives;
using PlatformPlatform.AccountManagement.Domain.Tenants;
using PlatformPlatform.AccountManagement.Infrastructure;

namespace PlatformPlatform.AccountManagement.Tests;

public class DatabaseSeeder
{
    public const string Tenant1Name = "Tenant 1";
    public const string Tenant2Name = "Tenant 2";
    public static readonly TenantId Tenant1Id = TenantId.NewId();
    public static readonly TenantId Tenant2Id = TenantId.NewId();

    private static readonly object Lock = new();
    private static bool _databaseIsSeeded;

    private readonly ApplicationDbContext _applicationDbContext;

    public DatabaseSeeder(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public void Seed()
    {
        lock (Lock)
        {
            if (_databaseIsSeeded) return;

            SeedTenants();

            _applicationDbContext.SaveChanges();

            _databaseIsSeeded = true;
        }
    }

    private void SeedTenants()
    {
        var tenant1 = new Tenant {Id = Tenant1Id, Name = Tenant1Name};
        ((IAuditableEntity) tenant1).SetCreatedAt(DateTime.UtcNow.Date);

        var tenant2 = new Tenant {Id = Tenant2Id, Name = Tenant2Name};
        ((IAuditableEntity) tenant2).SetCreatedAt(DateTime.UtcNow.Date);

        _applicationDbContext.Tenants.AddRange(tenant1, tenant2);
    }
}