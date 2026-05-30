using FlowCore.Application.Features.LeaveRequests.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.LeaveRequests.Queries
{
    public class GetLeaveRequestByIdQuery : IRequest<LeaveRequestDto?>
    {
        public Guid Id { get; set; }
    }
    public class GetLeaveRequestByIdQueryHandler : IRequestHandler<GetLeaveRequestByIdQuery, LeaveRequestDto?>
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        public GetLeaveRequestByIdQueryHandler(IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<LeaveRequestDto?> Handle(GetLeaveRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.Table
                .Include(lr => lr.User).ThenInclude(u => u!.Department)
                .FirstOrDefaultAsync(lr => lr.Id == request.Id && !lr.IsDeleted, cancellationToken);
            if (leaveRequest == null) return null;

            return new LeaveRequestDto
            {
                Id = leaveRequest.Id,
                UserId = leaveRequest.UserId,
                EmployeeFullName = leaveRequest.User?.FullName ?? string.Empty,
                DepartmentName = leaveRequest.User?.Department?.DepartmentName ?? string.Empty,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                TotalDays = (leaveRequest.EndDate - leaveRequest.StartDate).Days + 1,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status.ToString(),
                CreatedAt = leaveRequest.CreatedAt
            };

        }
    }
}
