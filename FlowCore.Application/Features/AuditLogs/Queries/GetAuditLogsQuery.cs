using FlowCore.Application.Features.AuditLogs.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.AuditLogs.Queries
{
    public class GetAuditLogsQuery : IRequest<List<AuditLogDto>>
    {
        public Guid? UserId { get; set; }
        public string? EntityName { get; set; }
        public Guid? EntityId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, List<AuditLogDto>>
    {
        private readonly IRepository<AuditLog> _auditLogRepository;

        public GetAuditLogsQueryHandler(IRepository<AuditLog> auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<List<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
        {
            var query = _auditLogRepository.Table
                .Include(a => a.User)
                .AsQueryable();

            if (request.UserId.HasValue)
                query = query.Where(a => a.UserId == request.UserId);

            if (!string.IsNullOrWhiteSpace(request.EntityName))
                query = query.Where(a => a.EntityName == request.EntityName);

            if (request.EntityId.HasValue)
                query = query.Where(a => a.EntityId == request.EntityId);

            if (request.FromDate.HasValue)
                query = query.Where(a => a.CreatedAt >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(a => a.CreatedAt <= request.ToDate.Value);

            var logs = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);

            return logs.Select(a => new AuditLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserFullName = a.User?.FullName ?? "Bilinmeyen Kullanıcı",
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                CreatedAt = a.CreatedAt
            }).ToList();
        }
    }
}
