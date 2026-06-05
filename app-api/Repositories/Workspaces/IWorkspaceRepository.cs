using System.Collections;
using app_api.Models;

public interface IWorkspaceRepository
{
    Task<List<Workspace>> GetAllWorkspacesAsync();
    Task<List<Workspace>?> GetWorkspacesByUserIdAsync(Guid userId);
    Task<Workspace?> GetWorkspaceByIdAsync(Guid workspaceId);
    Task<Workspace?> CreateWorkspaceAsync(Guid OwnerId, Workspace newWorkspace);
    Task<Workspace> UpdateWorkspaceAsync(Guid WorkspaceId, Workspace updatedWorkspace);
    Task DeleteWorkspaceAsync(Guid WorkspaceId);
}