using UserPermissions.Domain.Entities;

namespace UserPermissions.Infrastructure.Elasticsearch
{
    public interface IElasticsearchService
    {
        Task IndexPermissionAsync(Permission permission);
    }
}