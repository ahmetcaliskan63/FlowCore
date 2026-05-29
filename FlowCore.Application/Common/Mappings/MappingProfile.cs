using AutoMapper;
using FlowCore.Application.Features.Departments.DTOs;
using FlowCore.Application.Features.Roles.DTOs;
using FlowCore.Application.Features.Users.DTOs;
using FlowCore.Core.Entities;

namespace FlowCore.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Role, RoleDto>();
            CreateMap<Department, DepartmentDto>();
            CreateMap<User, UserDto>();
        }
    }
}
