namespace UserPermissions.Application.DTOs
{
    public class PermissionDTO
    {
        public int PermissionID { get; set; }
        public int EmployeeID { get; set; }
        public int PermissionTypeID { get; set; }
        public DateTime RequestDate { get; set; }
        public EmployeeDTO Employee { get; set; }
        public PermissionTypeDTO PermissionType { get; set; }
    }
}