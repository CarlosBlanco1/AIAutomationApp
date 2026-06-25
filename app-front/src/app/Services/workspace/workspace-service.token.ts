import { InjectionToken } from "@angular/core";
import { WorkspaceService } from "./workspace-service.interface";

export const WORKSPACE_SERVICE = new InjectionToken<WorkspaceService>("WORKSPACE_SERVICE")