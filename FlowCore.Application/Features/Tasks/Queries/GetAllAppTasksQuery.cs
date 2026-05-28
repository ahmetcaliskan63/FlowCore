using FlowCore.Application.Features.Tasks.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Tasks.Queries
{
    public class GetAllAppTasksQuery : IRequest<List<AppTaskDto>>
    {
        public Guid? AssignedToUserId { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public string? Status { get; set; }
    }

    public class GetAllAppTasksQueryHandler : IRequestHandler<GetAllAppTasksQuery, List<AppTaskDto>>
    {
        private readonly IRepository<AppTask> _taskRepository;

        public GetAllAppTasksQueryHandler(IRepository<AppTask> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<AppTaskDto>> Handle(GetAllAppTasksQuery request, CancellationToken cancellationToken)
        {
            var query = _taskRepository.Table
                .Where(t => !t.IsDeleted)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .AsQueryable();

            if (request.AssignedToUserId.HasValue)
                query = query.Where(t => t.AssignedToUserId == request.AssignedToUserId);

            if (request.CreatedByUserId.HasValue)
                query = query.Where(t => t.CreatedByUserId == request.CreatedByUserId);

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                Enum.TryParse(request.Status, true, out AppTaskStatus statusFilter))
                query = query.Where(t => t.Status == statusFilter);

            var tasks = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);

            return tasks.Select(t => new AppTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CreatedByUserId = t.CreatedByUserId,
                CreatedByUserName = t.CreatedByUser?.FullName ?? string.Empty,
                AssignedToUserId = t.AssignedToUserId,
                AssignedToUserName = t.AssignedToUser?.FullName ?? string.Empty,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
    }
}
