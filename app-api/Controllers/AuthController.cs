using System.Security.Claims;
using System.Text;
using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly ITokenRepository tokenRepository;
    private readonly IEmailSenderRepository emailSenderRepository;
    private readonly IRefreshTokenCoordinator refreshTokenCoordinator;
    private readonly IUserRepository userRepository;

    public AuthController(UserManager<User> userManager, ITokenRepository tokenRepository, IEmailSenderRepository emailSenderRepository, IRefreshTokenCoordinator refreshTokenCoordinator, IUserRepository userRepository)
    {
        this.userManager = userManager;
        this.tokenRepository = tokenRepository;
        this.emailSenderRepository = emailSenderRepository;
        this.refreshTokenCoordinator = refreshTokenCoordinator;
        this.userRepository = userRepository;
    }

    // POST api/Auth/Register
    [HttpPost]
    [Route("Register")]
    [ValidateModel]
    public async Task<IActionResult> Register([FromBody] CreateUserDTO createUserDTO)
    {
        var modelUser = new User()
        {
            UserName = createUserDTO.Email,
            FirstName = createUserDTO.FirstName,
            LastName = createUserDTO.LastName,
            Email = createUserDTO.Email,
            CreatedAt = DateOnly.FromDateTime(DateTime.Now)
        };

        var identityResult = await userManager.CreateAsync(modelUser, createUserDTO.Password);

        if (identityResult.Succeeded)
        {
            await emailSenderRepository.SendEmailAsync(await emailSenderRepository.CreateConfirmationEmailAsync(modelUser));
            return Ok("User was created succesfully!");
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

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
    {
        var user = await userManager.FindByEmailAsync(loginRequestDTO.Email);

        if (user == null)
        {
            return BadRequest("User with email not found!");
        }

        var isPasswordCorrect = await userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

        if (isPasswordCorrect == false)
        {
            return BadRequest("Password doesn't match!");
        }

        var refreshTokenResult = await refreshTokenCoordinator.CreateOrUpdateRefreshTokenForUser(user.Id);

        Response.Cookies.Append("workspace_ai_refresh_token", refreshTokenResult!.RawRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, //SET TO TRUEE for Https only
            SameSite = SameSiteMode.Strict, //NOT SURE IF IT'LL WORK, SAME DOMAIN SAME SITE?
            Expires = refreshTokenResult.ExpiresAt,
            Path = "/api/Auth/Refresh"
        });

        var response = new LoginResponseDTO()
        {
            JwtToken = tokenRepository.CreateJWTToken(user)
        };

        return Ok(response);
    }

    [HttpPost]
    [Route("Logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            await refreshTokenCoordinator.ClearUserRefreshToken(idInToken);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        Response.Cookies.Delete("workspace_ai_refresh_token", new CookieOptions
        {
            Path = "/api/Auth/Refresh"
        });

        return NoContent();
    }
    [HttpPost]
    [Route("Refresh")]
    public async Task<IActionResult> RefreshAccessToken()
    {
        try
        {
            var rawRefreshToken = Request.Cookies["workspace_ai_refresh_token"];

            var refreshTokenIssueResult = await refreshTokenCoordinator.ValidateAndRotateRefreshToken(rawRefreshToken!);

            Response.Cookies.Append("workspace_ai_refresh_token", refreshTokenIssueResult.RawRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = false, //CHANGE ON PROD
                Expires = refreshTokenIssueResult.ExpiresAt,
                Path = "/api/Auth/Refresh"
            });

            var user = await userRepository.GetUserByIdAsync(refreshTokenIssueResult.UserId);

            var response = new
            {
                JwtToken = tokenRepository.CreateJWTToken(user!)
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}