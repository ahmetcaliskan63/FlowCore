using FlowCore.Application.Features.Approvals.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Approvals.Commands
{
    public class ApproveLeaveRequestCommand : IRequest<LeaveApprovalResultDto>
    {
        public Guid LeaveRequestId { get; set; }
        public Guid ApproverUserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }

    public class ApproveLeaveRequestCommandHandler : IRequestHandler<ApproveLeaveRequestCommand, LeaveApprovalResultDto>
    {
        private readonly IRepository<Approval> _approvalRepository;
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        private readonly IRepository<User> _userRepository;

        public ApproveLeaveRequestCommandHandler(
                  IRepository<Approval> approvalRepository,
                  IRepository<LeaveRequest> leaveRequestRepository,
                  IRepository<User> userRepository)
        {
            _approvalRepository = approvalRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _userRepository = userRepository;
        }

        public async Task<LeaveApprovalResultDto> Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.Table
                .FirstOrDefaultAsync(lr => lr.Id == request.LeaveRequestId, cancellationToken);

            if (leaveRequest == null)
            {
                throw new Exception("Onaylanmak istenen izin talebi sistemde bulunamadı.");
            }

            if (request.ApproverUserId == leaveRequest.UserId)
            {
                throw new Exception("Personel kendi oluşturduğu izin talebini onaylayamaz veya reddedemez!");
            }

            if (leaveRequest.Status != ProcessStatus.OnayBekliyor && leaveRequest.Status != ProcessStatus.YoneticiOnayladi)
            {
                throw new Exception("Bu izin talebi daha önce zaten kesin olarak sonuçlandırılmış.");
            }

            var approverUser = await _userRepository.Table
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == request.ApproverUserId, cancellationToken);

            if (approverUser == null)
            {
                throw new Exception("Onaylama işlemini yapan kullanıcı sistemde bulunamadı.");
            }

            if (approverUser.Role?.Name != "Manager" && approverUser.Role?.Name != "HR")
            {
                throw new Exception("Bu işlemi gerçekleştirirmek için yetkiniz bulunmamaktadır. Sadece Manager veya HR onay verebilir.");
            }

            if (leaveRequest.Status == ProcessStatus.OnayBekliyor && approverUser.Role?.Name == "HR")
            {
                throw new Exception("Bu izin talebi henüz departman yöneticisi tarafından onaylanmamış. Önce Manager'ın onaylaması gerekir.");
            }

            if (leaveRequest.Status == ProcessStatus.YoneticiOnayladi && approverUser.Role?.Name == "Manager")
            {
                throw new Exception("Departman yöneticisi (Manager) bu izne zaten onay vermiş. Şu an HR onayı bekleniyor.");
            }

            ProcessStatus finalStatus;
            if (!request.IsApproved)
            {
                finalStatus = ProcessStatus.Reddedildi;
            }
            else
            {
                if (approverUser.Role?.Name == "Manager")
                {
                    finalStatus = ProcessStatus.YoneticiOnayladi;
                }
                else
                {
                    finalStatus = ProcessStatus.Onaylandi;
                }
            }

            var user = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.Id == leaveRequest.UserId, cancellationToken);

            if (user == null)
            {
                throw new Exception("İzin talebinde bulunan kullanıcı sistemde bulunamadı.");
            }

            int leaveDays = (leaveRequest.EndDate.Date - leaveRequest.StartDate.Date).Days + 1;

            if (finalStatus == ProcessStatus.Onaylandi)
            {
                if (user.RemainingLeaveCredits < leaveDays)
                {
                    throw new Exception("Kullanıcının kalan izin kredisi yetersiz!");
                }
                user.RemainingLeaveCredits -= leaveDays;
                await _userRepository.UpdateAsync(user);
            }

            leaveRequest.Status = finalStatus;
            await _leaveRequestRepository.UpdateAsync(leaveRequest);

            var approvalLog = new Approval
            {
                Id = Guid.NewGuid(),
                RequestType = WorkflowType.IzinTalebi,
                RequestId = leaveRequest.Id,
                ApproverByUserId = request.ApproverUserId,
                Status = finalStatus,
                Comment = request.Comment,
                ApprovedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.ApproverUserId
            };
            await _approvalRepository.AddAsync(approvalLog);

            return new LeaveApprovalResultDto
            {
                ApprovalId = approvalLog.Id,
                LeaveRequestId = leaveRequest.Id,
                NewStatus = finalStatus.ToString(),
                RemainingLeaveCredits = user.RemainingLeaveCredits,
                Message = finalStatus == ProcessStatus.Reddedildi ? "İzin talebi reddedildi." : (finalStatus == ProcessStatus.YoneticiOnayladi ? "İzin talebi yönetici tarafından onaylandı, İK onayı bekleniyor." : "İzin talebi İK tarafından onaylandı ve kesinleşti."),
                ActionAt = approvalLog.CreatedAt
            };
        }
    }
}