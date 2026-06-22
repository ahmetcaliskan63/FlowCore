using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Users.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;

        public DeleteUserCommandHandler(
            IRepository<User> userRepository,
            IRepository<LeaveRequest> leaveRequestRepository)
        {
            _userRepository = userRepository;
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li aktif bir kullanıcı bulunamadı.");

            var hasPendingLeaveRequests = await _leaveRequestRepository.Table
                .AnyAsync(lr => lr.UserId == request.Id && !lr.IsDeleted && 
                    (lr.Status == Core.Enums.ProcessStatus.OnayBekliyor ||
                     lr.Status == Core.Enums.ProcessStatus.YoneticiOnayladi), cancellationToken);

            if (hasPendingLeaveRequests)
                throw new BusinessException("Kullanıcının bekleyen izin talepleri bulunmaktadır. Önce izin taleplerini sonuçlandırın.");

            user.IsActive = false;
            user.IsDeleted = true;

            await _userRepository.UpdateAsync(user);

            return true;
        }
    }
}
