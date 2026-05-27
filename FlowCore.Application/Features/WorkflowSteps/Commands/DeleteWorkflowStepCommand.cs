using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.WorkflowSteps.Commands
{
    public class DeleteWorkflowStepCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid DeletedByUserId { get; set; }
    }

    public class DeleteWorkflowStepCommandHandler : IRequestHandler<DeleteWorkflowStepCommand, bool>
    {
        private readonly IRepository<WorkflowStep> _workflowStepRepository;

        public DeleteWorkflowStepCommandHandler(IRepository<WorkflowStep> workflowStepRepository)
        {
            _workflowStepRepository = workflowStepRepository;
        }

        public async Task<bool> Handle(DeleteWorkflowStepCommand request, CancellationToken cancellationToken)
        {
            var step = await _workflowStepRepository.Table
                .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

            if (step == null)
                throw new Exception("Silinmek istenen aktif bir iş akışı adımı bulunamadı.");

            step.IsDeleted = true;
            step.DeletedAt = DateTime.UtcNow;
            step.DeletedBy = request.DeletedByUserId;

            await _workflowStepRepository.UpdateAsync(step);

            return true;
        }
    }
}
