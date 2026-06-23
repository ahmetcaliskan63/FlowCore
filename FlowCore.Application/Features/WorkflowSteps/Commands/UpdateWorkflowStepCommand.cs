using FlowCore.Application.Features.WorkflowSteps.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.WorkflowSteps.Commands
{
    public class UpdateWorkflowStepCommand : IRequest<WorkflowStepsDTO>
    {
        public Guid Id { get; set; }
        public int StepOrder { get; set; }
        public Guid? RequiredRoleId { get; set; }
        public string ActionType { get; set; } = string.Empty;
    }

    public class UpdateWorkflowStepCommandHandler : IRequestHandler<UpdateWorkflowStepCommand, WorkflowStepsDTO>
    {
        private readonly IRepository<WorkflowStep> _workflowStepRepository;

        public UpdateWorkflowStepCommandHandler(IRepository<WorkflowStep> workflowStepRepository)
        {
            _workflowStepRepository = workflowStepRepository;
        }

        public async Task<WorkflowStepsDTO> Handle(UpdateWorkflowStepCommand request, CancellationToken cancellationToken)
        {
            var step = await _workflowStepRepository.Table
                .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

            if (step == null)
                throw new Exception("Güncellenecek aktif bir iş akışı adımı bulunamadı.");

            if (!Enum.TryParse(request.ActionType, true, out ActionType validatedActionType))
                throw new Exception($"Geçersiz aksiyon tipi: '{request.ActionType}'. Geçerli tipler: Onayla, Reddet, GeriGonder, Yonlendir");

            var duplicateStep = await _workflowStepRepository.Table
                .FirstOrDefaultAsync(s => s.WorkflowId == step.WorkflowId
                    && s.StepOrder == request.StepOrder
                    && s.Id != request.Id
                    && !s.IsDeleted, cancellationToken);

            if (duplicateStep != null)
                throw new Exception($"Bu iş akışında {request.StepOrder}. sıra numarası başka bir adım tarafından kullanılmaktadır.");

            step.StepOrder = request.StepOrder;
            step.RequiredRoleId = request.RequiredRoleId;
            step.ActionType = validatedActionType;
            step.UpdatedAt = DateTime.UtcNow;

            await _workflowStepRepository.UpdateAsync(step);

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
