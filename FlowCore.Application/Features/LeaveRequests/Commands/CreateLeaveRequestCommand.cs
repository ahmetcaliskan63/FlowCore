using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IRepository<User> _userRepository;
        public CreateLeaveRequestCommandHandler(IRepository<LeaveRequest> leaveRequestRepository, IRepository<User> userRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _userRepository = userRepository;
        }

        public async Task<Guid> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            if (request.StartDate.Date < DateTime.UtcNow.Date)
            {
                throw new Exception("Geçmiş tarihe yönelik izin talebi oluşturulamaz.");
            } 
            if(request.EndDate < request.StartDate) {
                throw new Exception("Bitiş tarihi başlangıç tarihinden önce olamaz.");
            }
            var user = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
            if (user == null) {
                throw new Exception("İzin talebi için geçerli bir kullanıcı bulunamadı.");
            }
            int requestedDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;
            if(requestedDays >user.RemainingLeaveCredits) {
                throw new Exception("Kullanıcının yeterli izin kredisi yok.");
            }
            var leaveRequest = new LeaveRequest
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason,

                Status = Core.Enums.ProcessStatus.OnayBekliyor,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.UserId
            };
             await _leaveRequestRepository.AddAsync(leaveRequest);
            return leaveRequest.Id;
        }
    }
}
