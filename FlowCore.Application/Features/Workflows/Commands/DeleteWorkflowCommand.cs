using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Workflows.Commands
{
    public class DeleteWorkflowCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
    public class DeleteWorkflowCommandHandler : IRequestHandler<DeleteWorkflowCommand, bool>
    {
        private readonly IRepository<Workflow> _workflowRepository;
        public DeleteWorkflowCommandHandler(IRepository<Workflow> workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<bool> Handle(DeleteWorkflowCommand request, CancellationToken cancellationToken)
        {
            var workflow = await _workflowRepository.Table
                .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted, cancellationToken);
            if (workflow == null)
            {
                throw new Exception("Silinmek istenen aktif bir iş akışı bulunamadı.");
            }
            workflow.IsDeleted = true;
            workflow.UpdatedAt = DateTime.UtcNow;
            await _workflowRepository.UpdateAsync(workflow);
            return true;
        }
    }
}