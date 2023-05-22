using MediatR;
using PlatformPlatform.AccountManagement.Domain.Tenants;
using PlatformPlatform.Foundation.DomainModeling.Cqrs;

namespace PlatformPlatform.AccountManagement.Application.Tenants.Queries;

/// <summary>
///     The GetTenantQuery will retrieve a Tenant with the specified TenantId from the repository. The query will
///     be handled by <see cref="GetTenant.Handler" />. Returns the Tenant if found, otherwise a NotFound result.
/// </summary>
public static class GetTenant
{
    public sealed record Query(TenantId Id) : IRequest<QueryResult<Tenant>>;

    public sealed class Handler : IRequestHandler<Query, QueryResult<Tenant>>
    {
        private readonly ITenantRepository _tenantRepository;

        public Handler(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<QueryResult<Tenant>> Handle(Query request, CancellationToken cancellationToken)
        {
            var tenant = await _tenantRepository.GetByIdAsync(request.Id, cancellationToken);
            return tenant ?? QueryResult<Tenant>.Failure($"Tenant with id '{request.Id}' not found.");
        }
    }
}