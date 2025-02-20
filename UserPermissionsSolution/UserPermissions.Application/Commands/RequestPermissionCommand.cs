using MediatR;

namespace UserPermissions.Application.Commands
{
    public class RequestPermissionCommand : IRequest<bool>
    {
        public int EmployeeId { get; set; }
        public int PermissionTypeId { get; set; }
    }
}