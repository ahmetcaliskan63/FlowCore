using FlowCore.Application.Features.Workflows.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Workflows.Queries
{
    public class GetWorkflowByIdQuery : IRequest<WorkflowDto>
    {
        public Guid Id { get; set; }
    }
    public class GetWorkflowByIdQueryHandler : IRequestHandler<GetWorkflowByIdQuery, WorkflowDto>
    {
        private readonly IRepository<Workflow> _workflowRepository;
        public GetWorkflowByIdQueryHandler(IRepository<Workflow> workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<WorkflowDto> Handle(GetWorkflowByIdQuery request, CancellationToken cancellationToken)
        {
            var workflow = await _workflowRepository.Table
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);
            if (workflow == null || workflow.IsDeleted)
            {
                throw new Exception("İş akışı bulunamadı.");
            }
            return new WorkflowDto
            {
                Id = workflow.Id,
                WorkflowName = workflow.Name,
                Type = workflow.Type.ToString(),
                IsActive = workflow.IsActive
            };
        }
    }
}
