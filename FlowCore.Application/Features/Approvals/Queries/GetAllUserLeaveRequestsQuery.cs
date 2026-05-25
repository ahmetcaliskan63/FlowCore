using FlowCore.Application.Features.LeaveRequests.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Approvals.Queries
{
    public class GetAllUserLeaveRequestsQuery : IRequest<List<LeaveRequestDto>>
    {
    }
    public class GetAllUserLeaveRequestsQueryHandler : IRequestHandler<GetAllUserLeaveRequestsQuery, List<LeaveRequestDto>>
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        public GetAllUserLeaveRequestsQueryHandler(IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }
        public async Task<List<LeaveRequestDto>> Handle(GetAllUserLeaveRequestsQuery request, CancellationToken cancellationToken)
        {
            var allLeaveRequests = await _leaveRequestRepository.Table
                .Include(u => u.User)
                .ThenInclude(u => u.Department)
                .OrderByDescending(lr => lr.CreatedAt)
                .ToListAsync(cancellationToken);
            var dtolist = allLeaveRequests.Select(lr => new LeaveRequestDto
            {
                Id = lr.Id,
                UserId = lr.UserId,
                EmployeeFullName = lr.User?.FullName ?? "Bilinmeyen Personel",
                DepartmentName = lr.User?.Department?.DepartmentName ?? "Bilinmeyen Departman",
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                TotalDays = (int)(lr.EndDate.Date - lr.StartDate.Date).Days + 1,
                Reason = lr.Reason,
                Status = lr.Status.ToString(),
                CreatedAt = lr.CreatedAt
            }).ToList();
            return dtolist;
        }
    }
}
