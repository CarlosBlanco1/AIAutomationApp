using app_api.Models;
using Microsoft.EntityFrameworkCore;

public class SQLUserRepository : IUserRepository
{
    private readonly MydbContext _dbContext;

    public SQLUserRepository(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<User?> CreateUserAsync(User newUser)
    {
        var emailExists = await _dbContext.Users.AnyAsync(u => u.Email == newUser.Email);

        if(emailExists)
        {
            return null;
        }

        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();
        return newUser;
    }

    public async Task<User?> DeleteUserAsync(Guid userId)
    {
        var userToDelete = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if(userToDelete == null)
        {
            return null;
        }

        _dbContext.Users.Remove(userToDelete);
        await _dbContext.SaveChangesAsync();

        return userToDelete;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }

    public async Task<User?> UpdateUserAsync(Guid userId, User updatedUser)
    {
        var userToUpdate = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if(userToUpdate == null)
        {
            return null;
        }

        userToUpdate.FirstName = updatedUser.FirstName;
        userToUpdate.LastName = updatedUser.LastName;

        await _dbContext.SaveChangesAsync();

        return userToUpdate;
    }
}