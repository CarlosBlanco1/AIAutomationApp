using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

public class EmailRepository : IEmailSenderRepository
{
    private readonly IConfiguration _config;

    public EmailRepository(IConfiguration config)
    {
        _config = config;
    }
    public MimeMessage CreateConfirmationEmail(SendEmailDTO sendEmailDTO)
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(_config["EMAIL_USERNAME"]!));
        email.To.Add(MailboxAddress.Parse(sendEmailDTO.To));
        email.Subject = "Please confirm your account with WorkspaceAI";

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = sendEmailDTO.Body
        };

        return email;
    }

    public async Task SendEmailAsync(MimeMessage email)
    {
        using var smtp = new SmtpClient();

        var host = _config["EMAIL_HOST"];
        var port = _config.GetValue<int>("EMAIL_PORT");
        var username = _config["EMAIL_USERNAME"];
        var password = _config["EMAIL_PASSWORD"];

        if (string.IsNullOrWhiteSpace(host))
            throw new InvalidOperationException("EmailHost configuration is missing");
        if (port <= 0)
            throw new InvalidOperationException("Invalid Port configuration");
        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException("EmailUsername configuration is missing");
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("EmailPassword configuration is missing");

        smtp.Connect(host, port, SecureSocketOptions.StartTls);
        smtp.Authenticate(username, password);

        try
        {
            smtp.Send(email);
        }
        finally
        {
            smtp.Disconnect(true);
        }
    }
}