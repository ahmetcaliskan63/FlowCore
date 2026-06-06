using AutoMapper;
using FlowCore.Application.Features.Approvals.DTOs;
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
            CreateMap<User, UserDto>().ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName :"Departman Atanmamış"));
            CreateMap<Approval, ApprovalDto>()
                .ForMember(dest => dest.ApproverFullName,
                           opt => opt.MapFrom(src => src.ApproverByUser != null ? src.ApproverByUser.FullName : "Sistem"))
                .ForMember(dest => dest.ApproverRole,
                           opt => opt.MapFrom(src => src.ApproverByUser != null && src.ApproverByUser.Role != null ? src.ApproverByUser.Role.Name : "Rol Tanımsız"))
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
