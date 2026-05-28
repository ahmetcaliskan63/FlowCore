using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.AuditLogs.Commands
{
    public class CreateAuditLogCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }

    public class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, Guid>
    {
        private readonly IRepository<AuditLog> _auditLogRepository;

        public CreateAuditLogCommandHandler(IRepository<AuditLog> auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<Guid> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Action = request.Action,
                EntityName = request.EntityName,
                EntityId = request.EntityId,
                OldValue = request.OldValue,
                NewValue = request.NewValue,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.UserId,
                IsDeleted = false
            };

            await _auditLogRepository.AddAsync(auditLog);

            return auditLog.Id;
        }
    }
}
