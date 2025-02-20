using AutoMapper;
using UserPermissions.Application.DTOs;
using UserPermissions.Domain.Entities;

namespace UserPermissions.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Permission, PermissionDTO>();
            CreateMap<Employee, EmployeeDTO>();
            CreateMap<PermissionType, PermissionTypeDTO>();
        }
    }
}