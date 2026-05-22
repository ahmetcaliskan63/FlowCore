using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;

namespace FlowCore.Application.Features.LeaveRequests.Commands
{
    public class DeleteLeaveRequestCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
    }
    public class DeleteLeaveRequestCommandHandler : IRequestHandler<DeleteLeaveRequestCommand, Guid>
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        public DeleteLeaveRequestCommandHandler(IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<Guid> Handle(DeleteLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
            if (leaveRequest == null) throw new InvalidOperationException("İzin talebi bulunamadı");

            await _leaveRequestRepository.DeleteAsync(leaveRequest);

            return leaveRequest.Id;
        }
    }
}
