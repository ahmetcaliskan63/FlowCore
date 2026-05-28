using FlowCore.Application.Features.StatusHistories.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.StatusHistories.Queries
{
    public class GetStatusHistoriesQuery : IRequest<List<StatusHistoryDto>>
    {
        public Guid? EntityId { get; set; }
        public string? EntityType { get; set; }
    }

    public class GetStatusHistoriesQueryHandler : IRequestHandler<GetStatusHistoriesQuery, List<StatusHistoryDto>>
    {
        private readonly IRepository<StatusHistory> _statusHistoryRepository;

        public GetStatusHistoriesQueryHandler(IRepository<StatusHistory> statusHistoryRepository)
        {
            _statusHistoryRepository = statusHistoryRepository;
        }

        public async Task<List<StatusHistoryDto>> Handle(GetStatusHistoriesQuery request, CancellationToken cancellationToken)
        {
            var query = _statusHistoryRepository.Table
                .Include(s => s.ChangedByUser)
                .AsQueryable();

            if (request.EntityId.HasValue)
                query = query.Where(s => s.EntityId == request.EntityId);

            if (!string.IsNullOrWhiteSpace(request.EntityType) &&
                Enum.TryParse(request.EntityType, true, out WorkflowType entityType))
                query = query.Where(s => s.EntityType == entityType);

            var histories = await query
                .OrderByDescending(s => s.ChangedAt)
                .ToListAsync(cancellationToken);

            return histories.Select(s => new StatusHistoryDto
            {
                Id = s.Id,
                EntityType = s.EntityType.ToString(),
                EntityId = s.EntityId,
                OldStatus = s.OldStatus.ToString(),
                NewStatus = s.NewStatus.ToString(),
                ChangedByUserId = s.ChangedByUserId,
                ChangedByUserName = s.ChangedByUser?.FullName ?? "Sistem",
                ChangedAt = s.ChangedAt
            }).ToList();
        }
    }
}
