using Microsoft.EntityFrameworkCore;
using UserPermissions.Domain.Entities;
using UserPermissions.Domain.Interfaces;
using UserPermissions.Infrastructure.Data;

namespace UserPermissions.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
        {
            await _context.Permissions.AddAsync(permission, cancellationToken);
        }

        public async Task DeleteAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
            }
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _context.Permissions
                .Include(p => p.Employee)
                .Include(p => p.PermissionType)
                .ToListAsync();
        }

        public async Task<Permission> GetByIdAsync(int id)
        {
            return await _context.Permissions.FindAsync(id);
        }

        public Task UpdateAsync(Permission permission)
        {
            _context.Permissions.Update(permission);
            return Task.CompletedTask;
        }
    }
}