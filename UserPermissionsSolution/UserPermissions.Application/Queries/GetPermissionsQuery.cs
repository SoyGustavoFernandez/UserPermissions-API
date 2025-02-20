using MediatR;
using UserPermissions.Domain.Entities;

namespace UserPermissions.Application.Queries
{
    public class GetPermissionsQuery : IRequest<IEnumerable<Permission>>
    {
    }
}