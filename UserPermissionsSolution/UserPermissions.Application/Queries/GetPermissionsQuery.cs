using MediatR;
using UserPermissions.Application.DTOs;

namespace UserPermissions.Application.Queries
{
    public class GetPermissionsQuery : IRequest<IEnumerable<PermissionDTO>>
    {
    }
}