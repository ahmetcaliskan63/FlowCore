using FlowCore.Application.Features.Tasks.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Tasks.Queries
{
    public class GetAppTaskByIdQuery : IRequest<AppTaskDto>
    {
        public Guid Id { get; set; }
    }

    public class GetAppTaskByIdQueryHandler : IRequestHandler<GetAppTaskByIdQuery, AppTaskDto>
    {
        private readonly IRepository<AppTask> _taskRepository;

        public GetAppTaskByIdQueryHandler(IRepository<AppTask> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<AppTaskDto> Handle(GetAppTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.Table
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li aktif bir görev bulunamadı.");

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
