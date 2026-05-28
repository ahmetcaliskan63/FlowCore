using FlowCore.Application.Features.Notifications.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Notifications.Commands
{
    public class MarkNotificationAsReadCommand : IRequest<NotificationDto>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, NotificationDto>
    {
        private readonly IRepository<Notification> _notificationRepository;

        public MarkNotificationAsReadCommandHandler(IRepository<Notification> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<NotificationDto> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.Table
                .FirstOrDefaultAsync(n => n.Id == request.Id && !n.IsDeleted, cancellationToken);

            if (notification == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li bir bildirim bulunamadı.");

            if (notification.UserId != request.UserId)
                throw new UnauthorizedAccessException("Bu bildirimi okuma yetkiniz bulunmamaktadır.");

            if (notification.IsRead)
                return new NotificationDto
                {
                    Id = notification.Id,
                    UserId = notification.UserId,
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt
                };

            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;

            await _notificationRepository.UpdateAsync(notification);

            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}
