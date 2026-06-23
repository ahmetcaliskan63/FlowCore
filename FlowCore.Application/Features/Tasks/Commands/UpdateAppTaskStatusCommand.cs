using FlowCore.Core.Exceptions;
using FlowCore.Core.Constants;
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
    }

    public class UpdateAppTaskStatusCommandHandler : IRequestHandler<UpdateAppTaskStatusCommand, AppTaskDto>
    {
        private readonly IRepository<AppTask> _taskRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<User> _userRepository;

        public UpdateAppTaskStatusCommandHandler(
            IRepository<AppTask> taskRepository,
            ICurrentUserService currentUserService,
            IRepository<User> userRepository)
        {
            _taskRepository = taskRepository;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
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

            if (string.IsNullOrEmpty(_currentUserService.UserId) || !Guid.TryParse(_currentUserService.UserId, out var currentUserId))
            {
                throw new UnauthorizedException("Geçerli bir kullanıcı oturumu bulunamadı.");
            }

            var currentUser = await _userRepository.Table
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == currentUserId && !u.IsDeleted, cancellationToken);

            if (currentUser == null)
            {
                throw new UnauthorizedException("Geçerli bir kullanıcı oturumu bulunamadı.");
            }

            bool isAdmin = currentUser.Role?.Name == SystemRoles.Admin;
            bool isCreator = task.CreatedByUserId == currentUserId;
            bool isAssignee = task.AssignedToUserId == currentUserId;

            if (newStatus == AppTaskStatus.DevamEdiyor)
            {
                if (!isAssignee && !isAdmin && !isCreator)
                {
                    throw new UnauthorizedException("Sadece göreve atanan kişi (veya Admin/Görev Sahibi) görevi 'Devam Ediyor' durumuna çekebilir.");
                }
            }
            else if (newStatus == AppTaskStatus.Tamamlandi || newStatus == AppTaskStatus.IptalEdildi)
            {
                if (!isCreator && !isAdmin)
                {
                    throw new UnauthorizedException($"Görevi sadece onu oluşturan kişi veya sistem yöneticisi '{newStatus}' olarak işaretleyebilir.");
                }
            }

            task.Status = newStatus;
            task.UpdatedAt = DateTime.UtcNow;

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
