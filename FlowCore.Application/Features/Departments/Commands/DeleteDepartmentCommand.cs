using FlowCore.Core.Entities;
using FlowCore.Core.Exceptions;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Departments.Commands
{
    public class DeleteDepartmentCommand: IRequest<bool>
    {
        public Guid Id { get; set; }
    }
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<User> _userRepository;
        public DeleteDepartmentCommandHandler(IRepository<Department> departmentRepository, IRepository<User> userRepository)
        {
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository.Table
                .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted,cancellationToken);
            if (department == null) {
                throw new BusinessException("Silinmek istenen aktif bir departman bulunamadı.");
            }
            var hasActiveUsers = await _userRepository.Table
                .AnyAsync(u => u.DepartmentId == request.Id && !u.IsDeleted, cancellationToken);
            if(hasActiveUsers) {
                throw new BusinessException("Bu departmana bağlı aktif çalışan personeller bulunmaktadır. Departmanı silebilmek için önce personellerin departmanını değiştirmeniz gerekmektedir.");
            }
            department.IsDeleted = true;
            await _departmentRepository.UpdateAsync(department);
            return true;
        }
    }
}
