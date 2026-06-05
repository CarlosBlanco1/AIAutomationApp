using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using app_api.Models;
using Microsoft.IdentityModel.Tokens;

public class TokenRepository : ITokenRepository
{
    private readonly IConfiguration configuration;

    public TokenRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public string CreateJWTToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(configuration["Jwt:Issuer"], 
        configuration["Jwt:Audience"], 
        claims,
        expires : DateTime.Now.AddMinutes(15),
        signingCredentials : credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}