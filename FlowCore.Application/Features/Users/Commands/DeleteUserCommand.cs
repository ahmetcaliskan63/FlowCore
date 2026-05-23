using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Application.Features.Users.Commands
{
    public class DeleteUserCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Guid>
    {
        private readonly IRepository<User> _userRepository;
        public DeleteUserCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Guid> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) throw new Exception("User not found");
            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
            return user.Id;
        }
    }
}
