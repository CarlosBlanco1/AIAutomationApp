using app_api.Models;
using Microsoft.EntityFrameworkCore;

public class SQLWorkspaceRepository : IWorkspaceRepository
{
    private readonly MydbContext _dbContext;

    public SQLWorkspaceRepository(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Workspace?> CreateWorkspaceAsync(Guid OwnerId, Workspace newWorkspace)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == OwnerId);

        if(!userExists)
        {
            return null;
        }

        newWorkspace.OwnerId = OwnerId;

        await _dbContext.Workspaces.AddAsync(newWorkspace);
        await _dbContext.SaveChangesAsync();
        
        return newWorkspace;
    }

    public async Task DeleteWorkspaceAsync(Guid WorkspaceId)
    {
        var WorkspaceToDelete = await _dbContext.Workspaces.FirstAsync(u => u.WorkspaceId == WorkspaceId);

        _dbContext.Workspaces.Remove(WorkspaceToDelete);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Workspace>> GetAllWorkspacesAsync()
    {
        return await _dbContext.Workspaces.ToListAsync();
    }

    public async Task<Workspace?> GetWorkspaceByIdAsync(Guid workspaceId)
    {
        return await _dbContext.Workspaces.FirstOrDefaultAsync(w => w.WorkspaceId == workspaceId);
    }

    public async Task<List<Workspace>?> GetWorkspacesByUserIdAsync(Guid userId)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId);

        if(!userExists)
        {
            return null;
        }

        return await _dbContext.Workspaces.Where(w => w.OwnerId == userId).ToListAsync();
    }

    public async Task<Workspace> UpdateWorkspaceAsync(Guid WorkspaceId, Workspace updatedWorkspace)
    {
        var WorkspaceToUpdate = await _dbContext.Workspaces.FirstAsync(u => u.WorkspaceId == WorkspaceId);

        WorkspaceToUpdate.WorkspaceName = updatedWorkspace.WorkspaceName;

        await _dbContext.SaveChangesAsync();

        return WorkspaceToUpdate;
    }
}