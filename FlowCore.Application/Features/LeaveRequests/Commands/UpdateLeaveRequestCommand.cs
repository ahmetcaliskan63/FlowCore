using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.LeaveRequests.Commands
{
    public class UpdateLeaveRequestCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class UpdateLeaveRequestCommandHandler : IRequestHandler<UpdateLeaveRequestCommand, Guid>
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        public UpdateLeaveRequestCommandHandler(IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }
        public async Task<Guid> Handle(UpdateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
            if (leaveRequest == null) throw new InvalidOperationException("İzin talebi bulunamadı");
            leaveRequest.StartDate = request.StartDate;
            leaveRequest.EndDate = request.EndDate;
            leaveRequest.Reason = request.Reason;
            await _leaveRequestRepository.UpdateAsync(leaveRequest);
            return leaveRequest.Id;
        }
    }
}
