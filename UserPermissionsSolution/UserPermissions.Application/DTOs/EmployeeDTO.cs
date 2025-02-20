namespace UserPermissions.Application.DTOs
{
    public class EmployeeDTO
    {
        public int EmployeeID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string Email { get; set; }
    }
}