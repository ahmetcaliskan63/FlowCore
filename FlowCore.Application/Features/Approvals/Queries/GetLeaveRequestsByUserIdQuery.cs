using FlowCore.Application.Features.LeaveRequests.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Approvals.Queries
{
    public class GetLeaveRequestsByUserIdQuery : IRequest<List<LeaveRequestDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetLeaveRequestsByUserIdQueryHandler : IRequestHandler<GetLeaveRequestsByUserIdQuery, List<LeaveRequestDto>>
    {
        private readonly IRepository<LeaveRequest> _LeaveRequestRepository;
        public GetLeaveRequestsByUserIdQueryHandler(IRepository<LeaveRequest> LeaveRequestRepository)
        {
            _LeaveRequestRepository = LeaveRequestRepository;
        }
        public async Task<List<LeaveRequestDto>> Handle(GetLeaveRequestsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var leaveRequests = await _LeaveRequestRepository.Table
                .Include(lr => lr.User)
                .ThenInclude(u => u!.Department)
                .Where(lr => lr.UserId == request.UserId)
                .OrderByDescending(lr => lr.CreatedAt)
                .ToListAsync(cancellationToken);
            var dtolist = leaveRequests.Select(lr => new LeaveRequestDto
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
