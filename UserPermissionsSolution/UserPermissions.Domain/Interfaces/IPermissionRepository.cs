using UserPermissions.Domain.Entities;

namespace UserPermissions.Domain.Interfaces
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<Permission> GetByIdAsync(int id);
        Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
        Task UpdateAsync(Permission permission);
        Task DeleteAsync(int id);
    }
}