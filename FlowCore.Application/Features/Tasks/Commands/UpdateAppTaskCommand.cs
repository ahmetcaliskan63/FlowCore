using FlowCore.Application.Features.Tasks.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Tasks.Commands
{
    public class UpdateAppTaskCommand : IRequest<AppTaskDto>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? AssignedToUserId { get; set; }
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public Guid UpdatedByUserId { get; set; }
    }

    public class UpdateAppTaskCommandHandler : IRequestHandler<UpdateAppTaskCommand, AppTaskDto>
    {
        private readonly IRepository<AppTask> _taskRepository;
        private readonly IRepository<User> _userRepository;

        public UpdateAppTaskCommandHandler(
            IRepository<AppTask> taskRepository,
            IRepository<User> userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<AppTaskDto> Handle(UpdateAppTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.Table
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li aktif bir görev bulunamadı.");

            if (task.Status == AppTaskStatus.Tamamlandi || task.Status == AppTaskStatus.IptalEdildi)
                throw new InvalidOperationException("Tamamlanmış veya iptal edilmiş görevler güncellenemez.");

            if (!Enum.TryParse(request.Priority, true, out TaskPriority validatedPriority))
                throw new Exception($"Geçersiz öncelik seviyesi: '{request.Priority}'. Geçerli değerler: Dusuk, Orta, Yuksek, Acil");

            User? assignee = null;
            if (request.AssignedToUserId.HasValue)
            {
                assignee = await _userRepository.Table
                    .FirstOrDefaultAsync(u => u.Id == request.AssignedToUserId && !u.IsDeleted, cancellationToken);

                if (assignee == null)
                    throw new KeyNotFoundException("Görev atanacak kullanıcı bulunamadı.");
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.AssignedToUserId = request.AssignedToUserId;
            task.Priority = validatedPriority;
            task.DueDate = request.DueDate;
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
                AssignedToUserName = assignee?.FullName ?? string.Empty,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt
            };
        }
    }
}
