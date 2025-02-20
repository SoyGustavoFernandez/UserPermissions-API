using Microsoft.EntityFrameworkCore;
using UserPermissions.Domain.Entities;

namespace UserPermissions.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Permission>()
                .HasOne(p=> p.Employee)
                .WithMany(p=> p.Permissions)
                .HasForeignKey(p=> p.EmployeeID);

            modelBuilder.Entity<Permission>()
                .HasOne(p=> p.PermissionType)
                .WithMany(p=> p.Permissions)
                .HasForeignKey(p=> p.PermissionTypeID);
        }
    }
}