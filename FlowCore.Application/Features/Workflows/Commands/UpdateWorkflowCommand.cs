using FlowCore.Application.Features.Workflows.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Workflows.Commands
{
    public class UpdateWorkflowCommand : IRequest<WorkflowDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid UpdatedByUserId { get; set; }
    }

    public class UpdateWorkflowCommandHandler : IRequestHandler<UpdateWorkflowCommand, WorkflowDto>
    {
        private readonly IRepository<Workflow> _workflowRepository;

        public UpdateWorkflowCommandHandler(IRepository<Workflow> workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }

        public async Task<WorkflowDto> Handle(UpdateWorkflowCommand request, CancellationToken cancellationToken)
        {
            var workflow = await _workflowRepository.Table
                .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted, cancellationToken);

            if (workflow == null)
            {
                throw new Exception("Güncellenmek istenen aktif bir iş akışı bulunamadı.");
            }

            var duplicateWorkflow = await _workflowRepository.Table
                .FirstOrDefaultAsync(w => w.Name.ToLower() == request.Name.ToLower() && w.Id != request.Id && !w.IsDeleted, cancellationToken);

            if (duplicateWorkflow != null)
            {
                throw new Exception($"'{request.Name}' isimli bir iş akışı sistemde zaten mevcut.");
            }

            if (!Enum.TryParse(request.Type, true, out WorkflowType workflowType))
            {
                throw new Exception($"'{request.Type}' geçerli bir iş akışı türü değil. (Geçerli tipler: Gorev, IzinTalebi)");
            }

            workflow.Name = request.Name;
            workflow.Type = workflowType;
            workflow.UpdatedAt = DateTime.UtcNow;
            workflow.UpdatedBy = request.UpdatedByUserId;

            await _workflowRepository.UpdateAsync(workflow);

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