using AutoMapper;
using FlowCore.Application.Features.Users.DTOs;
using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Users.Queries
{
    public class GetAllUserQuery : IRequest<List<UserDto>>
    {
    }

    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, List<UserDto>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public GetAllUserQueryHandler(IRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.Table
                .Where(u=> !u.IsDeleted && u.IsActive)
                .OrderBy(u => u.FullName)
                .Select(u => _mapper.Map<UserDto>(u))
                .ToListAsync(cancellationToken);
        }
    }
}
