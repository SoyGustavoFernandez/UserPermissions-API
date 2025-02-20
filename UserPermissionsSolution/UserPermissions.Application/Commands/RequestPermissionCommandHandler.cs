using MediatR;
using UserPermissions.Domain.Entities;
using UserPermissions.Domain.Interfaces;

namespace UserPermissions.Application.Commands
{
    public class RequestPermissionCommandHandler : IRequestHandler<RequestPermissionCommand, bool>
    {
        private readonly IPermissionRepository _permissionRepository;

        public RequestPermissionCommandHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<bool> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = new Permission
            {
                EmployeeID = request.EmployeeId,
                PermissionTypeID = request.PermissionTypeId,
                RequestDate = DateTime.UtcNow
            };

            await _permissionRepository.AddAsync(permission);
            return true;
        }
    }
}