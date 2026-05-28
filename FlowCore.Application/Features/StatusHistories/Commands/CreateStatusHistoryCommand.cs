using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.StatusHistories.Commands
{
    public class CreateStatusHistoryCommand : IRequest<Guid>
    {
        public string EntityType { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public Guid? ChangedByUserId { get; set; }
    }

    public class CreateStatusHistoryCommandHandler : IRequestHandler<CreateStatusHistoryCommand, Guid>
    {
        private readonly IRepository<StatusHistory> _statusHistoryRepository;

        public CreateStatusHistoryCommandHandler(IRepository<StatusHistory> statusHistoryRepository)
        {
            _statusHistoryRepository = statusHistoryRepository;
        }

        public async Task<Guid> Handle(CreateStatusHistoryCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse(request.EntityType, true, out WorkflowType validatedEntityType))
                throw new Exception($"Geçersiz Entity Türü: {request.EntityType}");

            if (!Enum.TryParse(request.OldStatus, true, out ProcessStatus validatedOldStatus))
                throw new Exception($"Geçersiz Eski Durum: {request.OldStatus}");

            if (!Enum.TryParse(request.NewStatus, true, out ProcessStatus validatedNewStatus))
                throw new Exception($"Geçersiz Yeni Durum: {request.NewStatus}");

            var statusHistory = new StatusHistory
            {
                Id = Guid.NewGuid(),
                EntityType = validatedEntityType,
                EntityId = request.EntityId,
                OldStatus = validatedOldStatus,
                NewStatus = validatedNewStatus,
                ChangedByUserId = request.ChangedByUserId,
                ChangedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.ChangedByUserId ?? Guid.Empty,
                IsDeleted = false
            };

            await _statusHistoryRepository.AddAsync(statusHistory);

            return statusHistory.Id;
        }
    }
}
