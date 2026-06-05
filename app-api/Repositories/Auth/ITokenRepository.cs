using app_api.Models;

public interface ITokenRepository
{
    string CreateJWTToken(User user);
}