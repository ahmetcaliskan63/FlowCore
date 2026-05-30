using FlowCore.Application.Features.LeaveRequests.DTOs;
using Microsoft.EntityFrameworkCore;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;


namespace FlowCore.Application.Features.LeaveRequests.Queries
{
    public class GetAllLeaveRequestsQuery : IRequest<List<LeaveRequestDto>>
    {

    }
    public class GetAllLeaveRequestsQueryHandler : IRequestHandler<GetAllLeaveRequestsQuery, List<LeaveRequestDto>>
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        public GetAllLeaveRequestsQueryHandler(IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }
        public async Task<List<LeaveRequestDto>> Handle(GetAllLeaveRequestsQuery request, CancellationToken cancellationToken)
        {
            var leaveRequests = await _leaveRequestRepository.Table
                .Where(lr => !lr.IsDeleted)
                .Include(lr => lr.User)
                .ThenInclude(u => u!.Department)
                .OrderByDescending(lr => lr.CreatedAt)
                .ToListAsync(cancellationToken);
            return leaveRequests.Select(lr => new LeaveRequestDto
            {
                Id = lr.Id,
                UserId = lr.UserId,
                EmployeeFullName = lr.User?.FullName ?? "Bilinmeyen Personel",
                DepartmentName = lr.User?.Department?.DepartmentName ?? "Departman Atanmamış",
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                TotalDays = (lr.EndDate - lr.StartDate).Days + 1,
                Reason = lr.Reason,
                Status = lr.Status.ToString(),
                CreatedAt = lr.CreatedAt
            }).ToList();
        }
    }

}
