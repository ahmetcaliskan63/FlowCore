using FlowCore.Application.Features.Tasks.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Tasks.Commands
{
    public class CreateAppTaskCommand : IRequest<AppTaskDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CreatedByUserId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }

    public class CreateAppTaskCommandHandler : IRequestHandler<CreateAppTaskCommand, AppTaskDto>
    {
        private readonly IRepository<AppTask> _taskRepository;
        private readonly IRepository<User> _userRepository;

        public CreateAppTaskCommandHandler(
            IRepository<AppTask> taskRepository,
            IRepository<User> userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<AppTaskDto> Handle(CreateAppTaskCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse(request.Priority, true, out TaskPriority validatedPriority))
                throw new Exception($"Geçersiz öncelik seviyesi: '{request.Priority}'. Geçerli değerler: Dusuk, Orta, Yuksek, Acil");

            var creator = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.Id == request.CreatedByUserId && !u.IsDeleted, cancellationToken);

            if (creator == null)
                throw new KeyNotFoundException("Görevi oluşturan kullanıcı bulunamadı.");

            User? assignee = null;
            if (request.AssignedToUserId.HasValue)
            {
                assignee = await _userRepository.Table
                    .FirstOrDefaultAsync(u => u.Id == request.AssignedToUserId && !u.IsDeleted, cancellationToken);

                if (assignee == null)
                    throw new KeyNotFoundException("Görev atanacak kullanıcı bulunamadı.");
            }

            var task = new AppTask
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                CreatedByUserId = request.CreatedByUserId,
                AssignedToUserId = request.AssignedToUserId,
                Status = AppTaskStatus.Yapilacak,
                Priority = validatedPriority,
                DueDate = request.DueDate,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedByUserId,
                IsDeleted = false
            };

            await _taskRepository.AddAsync(task);

            return new AppTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedByUserId = task.CreatedByUserId,
                CreatedByUserName = creator.FullName,
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
