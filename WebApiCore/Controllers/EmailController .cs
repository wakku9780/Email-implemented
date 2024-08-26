using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCore.Models;
using WebApiCore.Services;

namespace WebApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly JobsService _jobsService;

        public EmailController(JobsService jobsService)
        {
            _jobsService = jobsService;
        }

        [HttpPost("send")]
        public IActionResult SendEmail([FromBody] EmailRequest emailRequest)
        {
            _jobsService.EnqueueEmail(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body);
            return Ok("Email queued successfully!");
        }
    }
}
