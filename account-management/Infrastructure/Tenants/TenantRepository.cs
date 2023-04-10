using Microsoft.EntityFrameworkCore;
using PlatformPlatform.AccountManagement.Domain.Tenants;

namespace PlatformPlatform.AccountManagement.Infrastructure.Tenants;

public class TenantRepository : ITenantRepository
{
    private readonly DbSet<Tenant> _tenantDbSet;

    public TenantRepository(ApplicationDbContext applicationDbContext)
    {
        _tenantDbSet = applicationDbContext.Tenants;
    }

    public async Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken cancellationToken)
    {
        return await _tenantDbSet.FindAsync(id, cancellationToken);
    }

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken)
    {
        await _tenantDbSet.AddAsync(tenant, cancellationToken);
    }
}