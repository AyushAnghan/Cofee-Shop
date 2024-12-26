using System.Net.Mail;

namespace NiceAdmin
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, List<Attachment> attachments);
    }
}
