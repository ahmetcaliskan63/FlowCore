using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowCore.Application.Features.Approvals.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Approvals.Queries
{
    public class GetApprovalsByRequestIdQuery : IRequest<List<ApprovalDto>>
    {
        public Guid RequestId { get; set; }
    }

    public class GetApprovalsByRequestIdQueryHandler : IRequestHandler<GetApprovalsByRequestIdQuery, List<ApprovalDto>>
    {
        private readonly IRepository<Approval> _approvalRepository;
        private readonly IMapper _mapper;

        public GetApprovalsByRequestIdQueryHandler(IRepository<Approval> approvalRepository, IMapper mapper)
        {
            _approvalRepository = approvalRepository;
            _mapper = mapper;
        }

        public async Task<List<ApprovalDto>> Handle(GetApprovalsByRequestIdQuery request, CancellationToken cancellationToken)
        {
            var approvals = await _approvalRepository.Table
                .Where(a=> a.RequestId == request.RequestId)
                .OrderBy(a => a.CreatedAt)
                .ProjectTo<ApprovalDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            if (approvals == null || approvals.Count == 0)
            {
                throw new KeyNotFoundException($"Hiç onay bulunamadı request ID: {request.RequestId}");
            }

            return approvals;
        }
    }
}
