using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.LeaveRequests.Commands
{
    public class DeleteLeaveRequestCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteLeaveRequestCommandHandler : IRequestHandler<DeleteLeaveRequestCommand, bool>
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;

        public DeleteLeaveRequestCommandHandler(IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<bool> Handle(DeleteLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.Table
                .FirstOrDefaultAsync(lr => lr.Id == request.Id && !lr.IsDeleted, cancellationToken);

            if (leaveRequest == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li aktif bir izin talebi bulunamadı.");

            if (leaveRequest.Status != ProcessStatus.OnayBekliyor)
                throw new InvalidOperationException("Yalnızca 'Onay Bekliyor' durumundaki izin talepleri silinebilir.");

            leaveRequest.IsDeleted = true;

            await _leaveRequestRepository.UpdateAsync(leaveRequest);

            return true;
        }
    }
}
