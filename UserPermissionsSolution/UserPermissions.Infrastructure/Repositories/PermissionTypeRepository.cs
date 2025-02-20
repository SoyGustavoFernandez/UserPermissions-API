using Microsoft.EntityFrameworkCore;
using UserPermissions.Domain.Entities;
using UserPermissions.Domain.Interfaces;
using UserPermissions.Infrastructure.Data;

namespace UserPermissions.Infrastructure.Repositories
{
    public class PermissionTypeRepository : IPermissionTypeRepository
    {
        private readonly AppDbContext _context;

        public PermissionTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PermissionType permissionType)
        {
            await _context.PermissionTypes.AddAsync(permissionType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var permissionType = _context.PermissionTypes.FindAsync(id);
            if(permissionType != null)
            {
                _context.Remove(permissionType);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PermissionType>> GetAllAsync()
        {
            return await _context.PermissionTypes.ToListAsync();
        }

        public async Task<PermissionType> GetByIdAsync(int id)
        {
            return await _context.PermissionTypes.FindAsync(id);
        }

        public async Task UpdateAsync(PermissionType permissionType)
        {
            _context.PermissionTypes.Update(permissionType);
            await _context.SaveChangesAsync();
        }
    }
}
