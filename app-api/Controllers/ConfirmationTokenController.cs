using System.Security.Claims;
using System.Text;
using app_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

[ApiController]
[Route("api/[controller]")]
public class ConfirmationTokenController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly IConfiguration configuration;
    private readonly IUserRepository userRepository;
    private readonly IEmailSenderRepository emailSender;

    public ConfirmationTokenController(UserManager<User> userManager, IConfiguration configuration, IUserRepository userRepository, IEmailSenderRepository emailSender)
    {
        this.userManager = userManager;
        this.configuration = configuration;
        this.userRepository = userRepository;
        this.emailSender = emailSender;
    }

    [HttpPost]
    [Route("email/generate")]
    [Authorize]
    public async Task<IActionResult> GenerateEmailConfirmationToken()
    {
        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound("User doesn't exist!");
        }
        else if (await userManager.IsEmailConfirmedAsync(user))
        {
            return BadRequest("User was already confirmed!");
        }

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

        var email = new SendEmailDTO
        {
            To = user.Email!,
            Body = emailMessage
        };

        try
        {
            await emailSender.SendEmailAsync(emailSender.CreateConfirmationEmail(email));
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [HttpGet]
    [Route("email")]
    [Authorize]
    public async Task<IActionResult> ValidateEmailConfirmationToken([FromQuery] Guid userId, [FromQuery] string token)
    {
        var user = await userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User doesn't exist");
        }

        IdentityResult identityResult = await userManager.ConfirmEmailAsync(user, token);

        if (identityResult.Succeeded)
        {
            return Ok("Email was validated successfully!");
        }
        else
        {
            StringBuilder errorString = new StringBuilder();

            foreach (var error in identityResult.Errors)
            {
                errorString.Append(error.Description);
            }

            return BadRequest(errorString.ToString());
        }
    }

}