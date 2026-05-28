using FlowCore.Application.Features.Notifications.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Notifications.Queries
{
    public class GetAllNotificationsQuery : IRequest<List<NotificationDto>>
    {
        public Guid UserId { get; set; }
        public bool? OnlyUnread { get; set; }
    }

    public class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, List<NotificationDto>>
    {
        private readonly IRepository<Notification> _notificationRepository;

        public GetAllNotificationsQueryHandler(IRepository<Notification> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<NotificationDto>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
        {
            var query = _notificationRepository.Table
                .Where(n => n.UserId == request.UserId && !n.IsDeleted);

            if (request.OnlyUnread == true)
                query = query.Where(n => !n.IsRead);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }
    }
}
