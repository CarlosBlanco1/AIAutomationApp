using MimeKit;

public interface IEmailSenderRepository
{
    Task SendEmailAsync(MimeMessage email);
    MimeMessage CreateConfirmationEmail(SendEmailDTO sendEmailDTO);
}