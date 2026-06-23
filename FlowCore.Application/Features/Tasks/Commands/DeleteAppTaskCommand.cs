using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Application.Features.Tasks.Commands
{
    public class DeleteAppTaskCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteAppTaskCommandHandler : IRequestHandler<DeleteAppTaskCommand, bool>
    {
        private readonly IRepository<AppTask> _taskRepository;

        public DeleteAppTaskCommandHandler(IRepository<AppTask> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<bool> Handle(DeleteAppTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.Table
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException($"'{request.Id}' ID'li aktif bir görev bulunamadı.");

            if (task.Status == Core.Enums.AppTaskStatus.DevamEdiyor)
                throw new InvalidOperationException("Devam eden bir görev silinemez. Önce görevi iptal edin veya tamamlayın.");

            task.IsDeleted = true;

            await _taskRepository.UpdateAsync(task);

            return true;
        }
    }
}
