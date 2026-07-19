using app_api.Models;
using MimeKit;

public interface IEmailSenderRepository
{
    Task SendEmailAsync(MimeMessage email);
    Task<MimeMessage> CreateConfirmationEmailAsync(User user);
}