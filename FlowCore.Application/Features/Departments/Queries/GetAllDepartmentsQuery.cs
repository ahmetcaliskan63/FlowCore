using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowCore.Application.Features.Departments.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Departments.Queries
{
    public class GetAllDepartmentsQuery : IRequest<List<DepartmentDto>>
    {
    }
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly IMapper _mapper;
        public GetAllDepartmentsQueryHandler(IRepository<Department> departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }
        public async Task<List<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await _departmentRepository.Table
                .Where(d => !d.IsDeleted)
                    .OrderBy(d => d.DepartmentName)
                    .ProjectTo<DepartmentDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            if (departments == null || departments.Count == 0)
                throw new KeyNotFoundException("Aktif bir departman bulunamadı.");
            return departments;
        }
    }

}