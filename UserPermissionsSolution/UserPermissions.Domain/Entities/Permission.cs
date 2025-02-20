namespace UserPermissions.Domain.Entities
{
    public class Permission
    {
        public int PermissionID { get; set; }
        public int EmployeeID { get; set; }
        public int PermissionTypeID { get; set; }
        public DateTime RequestDate { get; set; }

        public Employee Employee { get; set; }
        public PermissionType PermissionType { get; set; }
    }
}