import { Observable } from "rxjs";
import { WorkspaceDto } from "../../models/Workspaces/workspace-dto";
import { CreateWorkspaceRequest } from "../../models/Workspaces/create-workspace-request";

export interface WorkspaceService {
    userWorkspaces() : WorkspaceDto[]
    getUserWorkspaces() : Observable<WorkspaceDto[]>;
    createWorkspace(createWorkspaceRequest : CreateWorkspaceRequest) : Observable<void>;
}