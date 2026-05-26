using FlowCore.Application.Features.Workflows.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Workflows.Queries
{
    public class GetAllWorkflowQuery : IRequest<List<WorkflowDto>>
    {
    }
    public class GetAllWorkflowQueryHandler : IRequestHandler<GetAllWorkflowQuery, List<WorkflowDto>>
    {
        private readonly IRepository<Workflow> _workflowRepository;
        public GetAllWorkflowQueryHandler(IRepository<Workflow> workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<List<WorkflowDto>> Handle(GetAllWorkflowQuery request, CancellationToken cancellationToken)
        {
            var workflows = await _workflowRepository.Table
                .Where(w => !w.IsDeleted)
                .Select(w => new WorkflowDto
                {
                    Id = w.Id,
                    WorkflowName = w.Name,
                    Type = w.Type.ToString(),
                    IsActive = w.IsActive
                })
                .ToListAsync(cancellationToken);
            return workflows;
        }
    }
}
