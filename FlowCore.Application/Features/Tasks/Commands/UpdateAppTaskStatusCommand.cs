using FlowCore.Application.Features.Tasks.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Tasks.Commands
{
    public class UpdateAppTaskStatusCommand : IRequest<AppTaskDto>
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid UpdatedByUserId { get; set; }
    }

    public class UpdateAppTaskStatusCommandHandler : IRequestHandler<UpdateAppTaskStatusCommand, AppTaskDto>
    {
        private readonly IRepository<AppTask> _taskRepository;

        public UpdateAppTaskStatusCommandHandler(IRepository<AppTask> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<AppTaskDto> Handle(UpdateAppTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.Table
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li aktif bir görev bulunamadı.");

            if (!Enum.TryParse(request.Status, true, out AppTaskStatus newStatus))
                throw new Exception($"Geçersiz görev durumu: '{request.Status}'. Geçerli değerler: Yapilacak, DevamEdiyor, Tamamlandi, IptalEdildi");

            if (task.Status == AppTaskStatus.Tamamlandi)
                throw new InvalidOperationException("Tamamlanmış bir görevin durumu değiştirilemez.");

            if (task.Status == AppTaskStatus.IptalEdildi)
                throw new InvalidOperationException("İptal edilmiş bir görevin durumu değiştirilemez.");

            task.Status = newStatus;
            task.UpdatedAt = DateTime.UtcNow;
            task.UpdatedBy = request.UpdatedByUserId;

            await _taskRepository.UpdateAsync(task);

            return new AppTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedByUserId = task.CreatedByUserId,
                CreatedByUserName = task.CreatedByUser?.FullName ?? string.Empty,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser?.FullName ?? string.Empty,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt
            };
        }
    }
}
