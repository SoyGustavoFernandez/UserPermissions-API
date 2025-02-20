using MediatR;

namespace UserPermissions.Application.Commands
{
    public class ModifyPermissionCommand: IRequest<bool>
    {
        public int PermissionId { get; set; }
        public int EmployeeId { get; set; }
        public int PermissionTypeId { get; set; }
    }
}