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
    private readonly IUserRepository userRepository;
    private readonly IEmailSenderRepository emailSender;
    private readonly ITokenRepository tokenRepository;

    public ConfirmationTokenController(UserManager<User> userManager, IUserRepository userRepository, IEmailSenderRepository emailSender, ITokenRepository tokenRepository)
    {
        this.userManager = userManager;
        this.userRepository = userRepository;
        this.emailSender = emailSender;
        this.tokenRepository = tokenRepository;
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

        try
        {
            await emailSender.SendEmailAsync(await emailSender.CreateConfirmationEmailAsync(user));
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [HttpGet]
    [Route("email")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateEmailConfirmationToken([FromQuery] Guid userId, [FromQuery] string token)
    {
        var user = await userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User doesn't exist");
        }

        var webDecodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        IdentityResult identityResult = await userManager.ConfirmEmailAsync(user, webDecodedToken);

        if (identityResult.Succeeded)
        {
            var response = new
            {
                JwtToken = tokenRepository.CreateJWTToken(user)
            };

            return Ok(response);
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