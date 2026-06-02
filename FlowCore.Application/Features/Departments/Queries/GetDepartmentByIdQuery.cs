using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowCore.Application.Features.Departments.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Departments.Queries
{
    public class GetDepartmentByIdQuery : IRequest<DepartmentDto>
    {
        public Guid Id { get; set; }
    }
    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly IMapper _mapper;
        public GetDepartmentByIdQueryHandler(IRepository<Department> departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }
        public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository.Table
                .Where(d => d.Id == request.Id && !d.IsDeleted)
                .ProjectTo<DepartmentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
            if (department == null)
                throw new BusinessException($"'{request.Id}' ID'li bir departman bulunamadı.");
            return department;
        }
    }
}
