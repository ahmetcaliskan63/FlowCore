using FlowCore.Application.Features.Approvals.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Enums;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        async Task<LeaveApprovalResultDto> IRequestHandler<ApproveLeaveRequestCommand, LeaveApprovalResultDto>.Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.Table.FirstOrDefaultAsync(lr => lr.Id == request.LeaveRequestId, cancellationToken);
            if (leaveRequest == null)
            {
                throw new Exception("Onaylanmak istenen izin talebi sistemde bulunamadı.");
            }
            if(request.ApproverUserId == leaveRequest.UserId)
            {
                throw new Exception("Personel kendi oluşturduğu izin talebini onaylayamaz veya reddedemez!");
            }

            if (leaveRequest.Status != ProcessStatus.OnayBekliyor)
            {
                throw new Exception("Bu izin talebi daha önce zaten sonuçlandırılmış.");
            }
            var approverUser = await _userRepository.Table
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == request.ApproverUserId, cancellationToken);
            if(approverUser == null)
            {
                throw new Exception("Onaylama işlemini yapan yönetici sistemde bulunamadı.");
            }
            if(approverUser.Role?.Name != "Yönetici" && approverUser.Role?.Name != "İK")
    {
                throw new Exception("Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır. Sadece Yönetici veya İK onay verebilir.");
            }
            var finalStatus = request.IsApproved ? ProcessStatus.Onaylandi : ProcessStatus.Reddedildi;

            var user = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.Id == leaveRequest.UserId, cancellationToken);
            if (user == null)
            {
                throw new Exception("İzin talebinde bulunan kullanıcı sistemde bulunamadı.");
            }

            int leaveDays = (leaveRequest.EndDate.Date - leaveRequest.StartDate.Date).Days + 1;

            if (request.IsApproved)
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
                Message = request.IsApproved ? "İzin talebi onaylandı." : "İzin talebi reddedildi.",
                ActionAt = approvalLog.CreatedAt
            };
        }
    }
}