using UserPermissions.Domain.Entities;

namespace UserPermissions.Domain.Interfaces
{
    public interface IPermissionTypeRepository
    {
        Task<IEnumerable<PermissionType>> GetAllAsync();
        Task<PermissionType> GetByIdAsync(int id);
        Task AddAsync(PermissionType permissionType);
        Task UpdateAsync(PermissionType permissionType);
        Task DeleteAsync(int id);
    }
}