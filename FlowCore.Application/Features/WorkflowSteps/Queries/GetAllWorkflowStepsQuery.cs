using FlowCore.Application.Features.WorkflowSteps.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.WorkflowSteps.Queries
{
    public class GetAllWorkflowStepsQuery : IRequest<List<WorkflowStepsDTO>>
    {
        /// <summary>
        /// Belirli bir iş akışına ait adımları listelemek için opsiyonel filtre.
        /// Null bırakılırsa tüm aktif adımlar döner.
        /// </summary>
        public Guid? WorkflowId { get; set; }
    }

    public class GetAllWorkflowStepsQueryHandler : IRequestHandler<GetAllWorkflowStepsQuery, List<WorkflowStepsDTO>>
    {
        private readonly IRepository<WorkflowStep> _workflowStepRepository;

        public GetAllWorkflowStepsQueryHandler(IRepository<WorkflowStep> workflowStepRepository)
        {
            _workflowStepRepository = workflowStepRepository;
        }

        public async Task<List<WorkflowStepsDTO>> Handle(GetAllWorkflowStepsQuery request, CancellationToken cancellationToken)
        {
            var query = _workflowStepRepository.Table
                .Where(s => !s.IsDeleted);

            if (request.WorkflowId.HasValue)
                query = query.Where(s => s.WorkflowId == request.WorkflowId.Value);

            var steps = await query
                .OrderBy(s => s.WorkflowId)
                .ThenBy(s => s.StepOrder)
                .ToListAsync(cancellationToken);

            return steps.Select(s => new WorkflowStepsDTO
            {
                Id = s.Id,
                WorkflowId = s.WorkflowId,
                StepOrder = s.StepOrder,
                RequiredRoleId = s.RequiredRoleId,
                ActionType = s.ActionType.ToString()
            }).ToList();
        }
    }
}
