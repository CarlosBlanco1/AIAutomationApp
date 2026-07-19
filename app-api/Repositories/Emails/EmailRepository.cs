using System.Text;
using app_api.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;
using MimeKit.Text;

public class EmailRepository : IEmailSenderRepository
{
    private readonly IConfiguration configuration;
    private readonly UserManager<User> userManager;

    public EmailRepository(IConfiguration configuration, UserManager<User> userManager)
    {
        this.configuration = configuration;
        this.userManager = userManager;
    }
    public async Task<MimeMessage> CreateConfirmationEmailAsync(User user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var webEncodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var frontendUrl = configuration["FRONTEND_URL"];

        var confirmationUrl = $"{frontendUrl}/confirm-email?userId={user.Id}&token={webEncodedToken}";

        var emailMessage = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset=""UTF-8"">
        </head>
        <body style=""margin:0;padding:0;background-color:#f5f5f5;font-family:Arial,Helvetica,sans-serif;"">
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                    <td align=""center"" style=""padding:40px 20px;"">
                        <table width=""500"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff;border:1px solid #e5e5e5;border-radius:8px;padding:40px;"">
                            <tr>
                                <td align=""center"">
                                    <h2 style=""margin:0;color:#222;"">Confirm your email</h2>
                                </td>
                            </tr>

                            <tr>
                                <td style=""padding-top:20px;color:#555;font-size:16px;line-height:1.6;"">
                                    Thank you for registering! Please confirm your email address by clicking the button below.
                                </td>
                            </tr>

                            <tr>
                                <td align=""center"" style=""padding:35px 0;"">
                                    <a href=""{confirmationUrl}""
                                    style=""
                                        background:#111827;
                                        color:#ffffff;
                                        text-decoration:none;
                                        padding:14px 28px;
                                        border-radius:6px;
                                        display:inline-block;
                                        font-weight:bold;
                                    "">
                                        Confirm Email
                                    </a>
                                </td>
                            </tr>

                            <tr>
                                <td style=""color:#777;font-size:14px;line-height:1.6;"">
                                    If the button doesn't work, copy and paste this link into your browser:
                                </td>
                            </tr>

                            <tr>
                                <td style=""padding-top:12px;word-break:break-all;font-size:13px;color:#4b5563;"">
                                    <a href=""{confirmationUrl}"" style=""color:#2563eb;"">
                                        {confirmationUrl}
                                    </a>
                                </td>
                            </tr>

                            <tr>
                                <td style=""padding-top:30px;color:#999;font-size:13px;"">
                                    If you didn't create an account, you can safely ignore this email.
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>";

        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(configuration["EMAIL_USERNAME"]!));
        email.To.Add(MailboxAddress.Parse(user.Email!));
        email.Subject = "Please confirm your account with WorkspaceAI";

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = emailMessage
        };

        return email;
    }

    public async Task SendEmailAsync(MimeMessage email)
    {
        using var smtp = new SmtpClient();

        var host = configuration["EMAIL_HOST"];
        var port = configuration.GetValue<int>("EMAIL_PORT");
        var username = configuration["EMAIL_USERNAME"];
        var password = configuration["EMAIL_PASSWORD"];

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