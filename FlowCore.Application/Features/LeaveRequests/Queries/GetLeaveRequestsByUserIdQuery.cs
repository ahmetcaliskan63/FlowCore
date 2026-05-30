using FlowCore.Application.Features.LeaveRequests.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.LeaveRequests.Queries
{
    public class GetLeaveRequestsByUserIdQuery : IRequest<List<LeaveRequestDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetLeaveRequestsByUserIdQueryHandler : IRequestHandler<GetLeaveRequestsByUserIdQuery, List<LeaveRequestDto>>
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;

        public GetLeaveRequestsByUserIdQueryHandler(IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<List<LeaveRequestDto>> Handle(GetLeaveRequestsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var leaveRequests = await _leaveRequestRepository.Table
                .Where(lr => lr.UserId == request.UserId && !lr.IsDeleted)
                .Include(lr => lr.User)
                .ThenInclude(u => u!.Department)
                .OrderByDescending(lr => lr.CreatedAt)
                .ToListAsync(cancellationToken);

            return leaveRequests.Select(lr => new LeaveRequestDto
            {
                Id = lr.Id,
                UserId = lr.UserId,
                EmployeeFullName = lr.User?.FullName ?? "Bilinmeyen Personel",
                DepartmentName = lr.User?.Department?.DepartmentName ?? "Bilinmeyen Departman",
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                TotalDays = (lr.EndDate.Date - lr.StartDate.Date).Days + 1,
                Reason = lr.Reason,
                Status = lr.Status.ToString(),
                CreatedAt = lr.CreatedAt
            }).ToList();
        }
    }
}
