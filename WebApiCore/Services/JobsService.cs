//namespace WebApiCore.Services
using Hangfire;
using WebApiCore.Services;

namespace WebApiCore.Services
{
    public class JobsService
    {
        private readonly EmailService _emailService;

        public JobsService(EmailService emailService)
        {
            _emailService = emailService;
        }

        public void EnqueueEmail(string toEmail, string subject, string body)
        {
            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(toEmail, subject, body));
        }
    }
}

