using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.LeaveRequests.Commands
{
    public class UpdateLeaveRequestCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Guid UpdatedByUserId { get; set; }
    }

    public class UpdateLeaveRequestCommandHandler : IRequestHandler<UpdateLeaveRequestCommand, Guid>
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        private readonly IRepository<User> _userRepository;

        public UpdateLeaveRequestCommandHandler(
            IRepository<LeaveRequest> leaveRequestRepository,
            IRepository<User> userRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _userRepository = userRepository;
        }

        public async Task<Guid> Handle(UpdateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.Table
                .FirstOrDefaultAsync(lr => lr.Id == request.Id && !lr.IsDeleted, cancellationToken);

            if (leaveRequest == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li bir izin talebi bulunamadı.");

            if (leaveRequest.Status != ProcessStatus.OnayBekliyor)
                throw new InvalidOperationException("Yalnızca 'Onay Bekliyor' durumundaki izin talepleri güncellenebilir.");

            if (request.StartDate.Date < DateTime.UtcNow.Date)
                throw new InvalidOperationException("Geçmiş tarihe yönelik izin talebi güncellenemez.");

            if (request.EndDate < request.StartDate)
                throw new InvalidOperationException("Bitiş tarihi başlangıç tarihinden önce olamaz.");

            int requestedDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;

            var user = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.Id == leaveRequest.UserId && !u.IsDeleted, cancellationToken);

            if (user != null && requestedDays > user.RemainingLeaveCredits)
                throw new InvalidOperationException($"Kullanıcının yeterli izin kredisi yok. Kalan: {user.RemainingLeaveCredits} gün, Talep edilen: {requestedDays} gün.");

            leaveRequest.StartDate = request.StartDate;
            leaveRequest.EndDate = request.EndDate;
            leaveRequest.Reason = request.Reason;
            leaveRequest.UpdatedAt = DateTime.UtcNow;
            leaveRequest.UpdatedBy = request.UpdatedByUserId;

            await _leaveRequestRepository.UpdateAsync(leaveRequest);

            return leaveRequest.Id;
        }
    }
}
