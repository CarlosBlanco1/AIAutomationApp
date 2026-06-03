// using app_api.Models;
// using Microsoft.EntityFrameworkCore;

// public class SQLWorkspaceRepository : IWorkspaceRepository
// {
//     private readonly MydbContext _dbContext;

//     public SQLWorkspaceRepository(MydbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }
//     public async Task<Workspace?> CreateWorkspaceAsync(Workspace newWorkspace)
//     {
//         var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == newWorkspace.OwnerId);

//         if(!userExists)
//         {
//             return null;
//         }

//         await _dbContext.Workspaces.AddAsync(newWorkspace);
//         await _dbContext.SaveChangesAsync();
        
//         return newWorkspace;
//     }

//     public async Task<Workspace?> DeleteWorkspaceAsync(Guid WorkspaceId)
//     {
//         var WorkspaceToDelete = await _dbContext.Workspaces.FirstOrDefaultAsync(u => u.WorkspaceId == WorkspaceId);

//         if(WorkspaceToDelete == null)
//         {
//             return null;
//         }

//         _dbContext.Workspaces.Remove(WorkspaceToDelete);
//         await _dbContext.SaveChangesAsync();

//         return WorkspaceToDelete;
//     }

//     public async Task<List<Workspace>> GetAllWorkspacesAsync()
//     {
//         return await _dbContext.Workspaces.ToListAsync();
//     }

//     public async Task<List<Workspace>?> GetWorkspacesByUserIdAsync(Guid userId)
//     {
//         var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == userId);

//         if(!userExists)
//         {
//             return null;
//         }

//         return await _dbContext.Workspaces.Where(w => w.OwnerId == userId).ToListAsync();
//     }

//     public async Task<Workspace?> UpdateWorkspaceAsync(Guid WorkspaceId, Workspace updatedWorkspace)
//     {
//         var WorkspaceToUpdate = await _dbContext.Workspaces.FirstOrDefaultAsync(u => u.WorkspaceId == WorkspaceId);

//         if(WorkspaceToUpdate == null)
//         {
//             return null;
//         }

//         WorkspaceToUpdate.WorkspaceName = updatedWorkspace.WorkspaceName;

//         await _dbContext.SaveChangesAsync();

//         return WorkspaceToUpdate;
//     }
// }