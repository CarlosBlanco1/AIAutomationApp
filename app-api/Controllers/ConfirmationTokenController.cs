using System.Text;
using app_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ConfirmationTokenController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly IUserRepository userRepository;
    private readonly IEmailSenderRepository emailSender;

    public ConfirmationTokenController(UserManager<User> userManager, IUserRepository userRepository, IEmailSenderRepository emailSender)
    {
        this.userManager = userManager;
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