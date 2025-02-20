using System.ComponentModel.DataAnnotations;

namespace UserPermissions.Domain.Entities
{
    public class PermissionType
    {
        public int PermissionTypeID { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }

        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}