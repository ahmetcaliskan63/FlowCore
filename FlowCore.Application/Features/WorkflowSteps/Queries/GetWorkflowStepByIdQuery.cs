using FlowCore.Application.Features.WorkflowSteps.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.WorkflowSteps.Queries
{
    public class GetWorkflowStepByIdQuery : IRequest<WorkflowStepsDTO>
    {
        public Guid Id { get; set; }
    }

    public class GetWorkflowStepByIdQueryHandler : IRequestHandler<GetWorkflowStepByIdQuery, WorkflowStepsDTO>
    {
        private readonly IRepository<WorkflowStep> _workflowStepRepository;

        public GetWorkflowStepByIdQueryHandler(IRepository<WorkflowStep> workflowStepRepository)
        {
            _workflowStepRepository = workflowStepRepository;
        }

        public async Task<WorkflowStepsDTO> Handle(GetWorkflowStepByIdQuery request, CancellationToken cancellationToken)
        {
            var step = await _workflowStepRepository.Table
                .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

            if (step == null)
                throw new Exception($"'{request.Id}' ID'li aktif bir iş akışı adımı bulunamadı.");

            return new WorkflowStepsDTO
            {
                Id = step.Id,
                WorkflowId = step.WorkflowId,
                StepOrder = step.StepOrder,
                RequiredRoleId = step.RequiredRoleId,
                ActionType = step.ActionType.ToString()
            };
        }
    }
}
