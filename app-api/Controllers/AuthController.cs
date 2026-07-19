using System.Text;
using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly ITokenRepository tokenRepository;
    private readonly IEmailSenderRepository emailSenderRepository;

    public AuthController(UserManager<User> userManager, ITokenRepository tokenRepository, IEmailSenderRepository emailSenderRepository)
    {
        this.userManager = userManager;
        this.tokenRepository = tokenRepository;
        this.emailSenderRepository = emailSenderRepository;
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

        var response = new LoginResponseDTO()
        {
            JwtToken = tokenRepository.CreateJWTToken(user)
        };

        return Ok(response);

    }
}