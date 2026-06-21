using FlowCore.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlowCore.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"[EMAIL MOCK] To: {to}, Subject: {subject}");
            return Task.CompletedTask;
        }
    }
}
