import { inject, Injectable, signal } from "@angular/core";
import { WorkspaceService } from "./workspace-service.interface";
import { map, Observable, tap } from "rxjs";
import { CreateWorkspaceRequest } from "../../models/Workspaces/create-workspace-request";
import { WorkspaceDto } from "../../models/Workspaces/workspace-dto";
import { HttpClient } from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class ApiWorkspaceService implements WorkspaceService {

    userWorkspaces = signal<WorkspaceDto[]>([])
    httpClient = inject(HttpClient)

    getUserWorkspaces(): Observable<WorkspaceDto[]> {
        return this.httpClient.get<WorkspaceDto[]>('http://localhost:8080/api/Workspace')
            .pipe(tap(res => { this.userWorkspaces.set(res) }))
    }

    createWorkspace(request: CreateWorkspaceRequest): Observable<void> {
        return this.httpClient.post<WorkspaceDto>('http://localhost:8080/api/Workspace', request)
            .pipe(map(() => void 0))
    }

}