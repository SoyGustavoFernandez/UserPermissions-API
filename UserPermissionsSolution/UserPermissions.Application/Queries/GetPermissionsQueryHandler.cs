using MediatR;
using UserPermissions.Domain.Entities;
using UserPermissions.Domain.Interfaces;

namespace UserPermissions.Application.Queries
{
    public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, IEnumerable<Permission>>
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetPermissionsQueryHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<IEnumerable<Permission>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            return await _permissionRepository.GetAllAsync();
        }
    }
}