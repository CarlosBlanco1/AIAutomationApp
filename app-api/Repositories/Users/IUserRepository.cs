using app_api.Models;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> CreateUserAsync(User newUser);
    Task<User?> UpdateUserAsync(Guid userId, User updatedUser);
    Task<User?> DeleteUserAsync(Guid userId);
}