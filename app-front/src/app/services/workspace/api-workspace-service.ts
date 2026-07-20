import { inject, Injectable, signal } from "@angular/core";
import { WorkspaceService } from "./workspace-service.interface";
import { map, Observable, tap } from "rxjs";
import { CreateWorkspaceRequest } from "../../models/Workspaces/create-workspace-request";
import { WorkspaceDto } from "../../models/Workspaces/workspace-dto";
import { HttpClient } from "@angular/common/http";
import { AppConfigService } from "../configuration/app-config.service";

@Injectable({ providedIn: 'root' })
export class ApiWorkspaceService implements WorkspaceService {
    private baseUrl? : string;

    userWorkspaces = signal<WorkspaceDto[]>([]);
    httpClient = inject(HttpClient);
    configService = inject(AppConfigService);

    constructor() {
        this.baseUrl = `${this.configService.apiUrl}/api/Workspace`;
    }

    getUserWorkspaces(): Observable<WorkspaceDto[]> {
        return this.httpClient.get<WorkspaceDto[]>(`${this.baseUrl}`)
            .pipe(tap(res => { this.userWorkspaces.set(res) }))
    }

    createWorkspace(request: CreateWorkspaceRequest): Observable<void> {
        return this.httpClient.post<WorkspaceDto>(`${this.baseUrl}`, request)
            .pipe(map(() => void 0))
    }

}