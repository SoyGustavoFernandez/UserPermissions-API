using MediatR;
using UserPermissions.Domain.Interfaces;

namespace UserPermissions.Application.Commands
{
    public class ModifyPermissionCommandHandler : IRequestHandler<ModifyPermissionCommand, bool>
    {
        private readonly IPermissionRepository _permissionRepository;

        public ModifyPermissionCommandHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<bool> Handle(ModifyPermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.GetByIdAsync(request.PermissionId);
            if(permission == null)
            {
                return false;
            }

            permission.EmployeeID = request.EmployeeId;
            permission.PermissionTypeID = request.PermissionTypeId;

            await _permissionRepository.UpdateAsync(permission);
            return true;
        }
    }
}