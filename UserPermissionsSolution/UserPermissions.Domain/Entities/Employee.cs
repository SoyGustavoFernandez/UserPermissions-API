namespace UserPermissions.Domain.Entities
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }

        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}