using FlowCore.Application.Features.WorkflowSteps.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.WorkflowSteps.Commands
{
    public class CreateWorkflowStepCommand : IRequest<WorkflowStepsDTO>
    {
        public Guid WorkflowId { get; set; }
        public int StepOrder { get; set; }
        public Guid? RequiredRoleId { get; set; }
        public string ActionType { get; set; } = string.Empty;
    }

    public class CreateWorkflowStepCommandHandler : IRequestHandler<CreateWorkflowStepCommand, WorkflowStepsDTO>
    {
        private readonly IRepository<WorkflowStep> _workflowStepRepository;
        private readonly IRepository<Workflow> _workflowRepository;

        public CreateWorkflowStepCommandHandler(
            IRepository<WorkflowStep> workflowStepRepository,
            IRepository<Workflow> workflowRepository)
        {
            _workflowStepRepository = workflowStepRepository;
            _workflowRepository = workflowRepository;
        }

        public async Task<WorkflowStepsDTO> Handle(CreateWorkflowStepCommand request, CancellationToken cancellationToken)
        {
            var workflow = await _workflowRepository.Table
                .FirstOrDefaultAsync(w => w.Id == request.WorkflowId && !w.IsDeleted, cancellationToken);

            if (workflow == null)
                throw new Exception($"'{request.WorkflowId}' ID'li aktif bir iş akışı bulunamadı.");

            if (!Enum.TryParse(request.ActionType, true, out ActionType validatedActionType))
                throw new Exception($"Geçersiz aksiyon tipi: '{request.ActionType}'. Geçerli tipler: Onayla, Reddet, GeriGonder, Yonlendir");

            var duplicateStep = await _workflowStepRepository.Table
                .FirstOrDefaultAsync(s => s.WorkflowId == request.WorkflowId
                    && s.StepOrder == request.StepOrder
                    && !s.IsDeleted, cancellationToken);

            if (duplicateStep != null)
                throw new Exception($"Bu iş akışında {request.StepOrder}. sıra numarası zaten kullanılmaktadır.");

            var step = new WorkflowStep
            {
                Id = Guid.NewGuid(),
                WorkflowId = request.WorkflowId,
                StepOrder = request.StepOrder,
                RequiredRoleId = request.RequiredRoleId,
                ActionType = validatedActionType,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _workflowStepRepository.AddAsync(step);

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
