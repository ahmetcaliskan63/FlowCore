using FlowCore.Application.Features.Approvals.DTOs;
using FlowCore.Application.Features.AuditLogs.Commands;
using FlowCore.Application.Features.StatusHistories.Commands;
using FlowCore.Core.Constants;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Exceptions;
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
        private readonly IMediator _mediator;

        public ApproveLeaveRequestCommandHandler(
                  IRepository<Approval> approvalRepository,
                  IRepository<LeaveRequest> leaveRequestRepository,
                  IRepository<User> userRepository,
                  IMediator mediator)
        {
            _approvalRepository = approvalRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<LeaveApprovalResultDto> Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.Table
                .FirstOrDefaultAsync(lr => lr.Id == request.LeaveRequestId && !lr.IsDeleted, cancellationToken);

            if (leaveRequest == null)
            {
                throw new BusinessException("Onaylanmak istenen izin talebi sistemde bulunamadı.");
            }

            if (request.ApproverUserId == leaveRequest.UserId)
            {
                throw new BusinessException("Personel kendi oluşturduğu izin talebini onaylayamaz veya reddedemez!");
            }

            if (leaveRequest.Status != ProcessStatus.OnayBekliyor && leaveRequest.Status != ProcessStatus.YoneticiOnayladi)
            {
                throw new BusinessException("Bu izin talebi daha önce zaten kesin olarak sonuçlandırılmış.");
            }

            var originalStatusStr = leaveRequest.Status.ToString();

            var approverUser = await _userRepository.Table
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == request.ApproverUserId && !u.IsDeleted, cancellationToken);

            if (approverUser == null)
            {
                throw new BusinessException("Onaylama işlemini yapan kullanıcı sistemde bulunamadı.");
            }

            if (approverUser.Role == null || !SystemRoles.CanApproveLeaveRequests.Contains(approverUser.Role.Name))
            {
                throw new BusinessException("Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır. Sadece Manager veya HR onay verebilir.");
            }

            if (leaveRequest.Status == ProcessStatus.OnayBekliyor && approverUser.Role.Name == SystemRoles.HR)
            {
                throw new BusinessException("Bu izin talebi henüz departman yöneticisi tarafından onaylanmamış. Önce Manager'ın onaylaması gerekir.");
            }

            if (leaveRequest.Status == ProcessStatus.YoneticiOnayladi && approverUser.Role.Name == SystemRoles.Manager)
            {
                throw new BusinessException("Departman yöneticisi (Manager) bu izne zaten onay vermiş. Şu an HR onayı bekleniyor.");
            }

            ProcessStatus finalStatus;
            if (!request.IsApproved)
            {
                finalStatus = ProcessStatus.Reddedildi;
            }
            else
            {
                finalStatus = approverUser.Role?.Name == SystemRoles.Manager
                    ? ProcessStatus.YoneticiOnayladi
                    : ProcessStatus.Onaylandi;
            }

            var user = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.Id == leaveRequest.UserId && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                throw new BusinessException("İzin talebinde bulunan kullanıcı sistemde bulunamadı.");
            }

            int leaveDays = (leaveRequest.EndDate.Date - leaveRequest.StartDate.Date).Days + 1;

            if (finalStatus == ProcessStatus.Onaylandi)
            {
                if (user.RemainingLeaveCredits < leaveDays)
                {
                    throw new BusinessException($"Kullanıcının kalan izin kredisi yetersiz! Gerekli: {leaveDays}, Mevcut: {user.RemainingLeaveCredits}");
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

            await _mediator.Send(new CreateStatusHistoryCommand
            {
                EntityType = WorkflowType.IzinTalebi.ToString(),
                EntityId = leaveRequest.Id,
                OldStatus = originalStatusStr,
                NewStatus = finalStatus.ToString(),
                ChangedByUserId = request.ApproverUserId
            }, cancellationToken);

            await _mediator.Send(new CreateAuditLogCommand
            {
                UserId = request.ApproverUserId,
                Action = "ApproveLeaveRequest",
                EntityName = "LeaveRequest",
                EntityId = leaveRequest.Id,
                OldValue = originalStatusStr,
                NewValue = finalStatus.ToString()
            }, cancellationToken);

            return new LeaveApprovalResultDto
            {
                ApprovalId = approvalLog.Id,
                LeaveRequestId = leaveRequest.Id,
                NewStatus = finalStatus.ToString(),
                RemainingLeaveCredits = user.RemainingLeaveCredits,
                Message = finalStatus == ProcessStatus.Reddedildi
                    ? "İzin talebi reddedildi."
                    : (finalStatus == ProcessStatus.YoneticiOnayladi
                        ? "İzin talebi yönetici tarafından onaylandı, İK onayı bekleniyor."
                        : "İzin talebi İK tarafından onaylandı ve kesinleşti."),
                ActionAt = approvalLog.CreatedAt
            };
        }
    }
}