using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Notifications.Commands
{
    public class MarkAllNotificationsAsReadCommand : IRequest<int>
    {
        public Guid UserId { get; set; }
    }

    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, int>
    {
        private readonly IRepository<Notification> _notificationRepository;

        public MarkAllNotificationsAsReadCommandHandler(IRepository<Notification> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<int> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var unreadNotifications = await _notificationRepository.Table
                .Where(n => n.UserId == request.UserId && !n.IsRead && !n.IsDeleted)
                .ToListAsync(cancellationToken);

            if (!unreadNotifications.Any())
                return 0;

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                await _notificationRepository.UpdateAsync(notification);
            }

            return unreadNotifications.Count;
        }
    }
}
