using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;

namespace FlowCore.Application.Features.LeaveRequests.Commands
{
    public class CreateLeaveRequestCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, Guid>
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        public CreateLeaveRequestCommandHandler(IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<Guid> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = new LeaveRequest
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason,

                Status = Core.Enums.ProcessStatus.OnayBekliyor,
                CreatedAt = DateTime.UtcNow
            };
            await _leaveRequestRepository.AddAsync(leaveRequest);
            return leaveRequest.Id;
        }
    }
}
