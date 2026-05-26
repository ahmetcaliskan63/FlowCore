using FlowCore.Application.Features.Workflows.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums; // WorkflowType enum'ı için eklendi
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Workflows.Commands
{
    public class CreateWorkflowCommand : IRequest<WorkflowDto>
    {
        public string WorkflowName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid CreatedByUserId { get; set; }
    }

    public class CreateWorkflowCommandHandler : IRequestHandler<CreateWorkflowCommand, WorkflowDto>
    {
        private readonly IRepository<Workflow> _workflowRepository;

        public CreateWorkflowCommandHandler(IRepository<Workflow> workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }

        public async Task<WorkflowDto> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
        {
            var existingWorkflow = await _workflowRepository.Table
                .FirstOrDefaultAsync(w => w.Name.ToLower() == request.WorkflowName.ToLower() && !w.IsDeleted, cancellationToken);

            if (existingWorkflow != null)
            {
                throw new Exception($"'{request.WorkflowName}' isimli bir iş akışı sistemde zaten mevcut.");
            }

            if (!Enum.TryParse(request.Type, true, out WorkflowType validatedType))
            {
                throw new Exception($"Geçersiz iş akışı tipi: '{request.Type}'. Geçerli tipler: Gorev, IzinTalebi");
            }

            var workflow = new Workflow
            {
                Id = Guid.NewGuid(),
                Name = request.WorkflowName,
                Type = validatedType,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedByUserId,
                IsDeleted = false
            };

            await _workflowRepository.AddAsync(workflow);

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