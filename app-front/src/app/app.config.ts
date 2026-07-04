import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { JwtAuthService } from './services/auth/jwt-auth.service';
import { AUTH_SERVICE } from './services/auth/auth-service.token';
import { USER_SERVICE } from './services/user/user-service.token';
import { ApiUserService } from './services/user/api-user-service';
import { authInterceptor } from './interceptors/auth.interceptor';
import { WORKSPACE_SERVICE } from './services/workspace/workspace-service.token';
import { ApiWorkspaceService } from './services/workspace/api-workspace-service';
import { NgxSmartModalModule } from 'ngx-smart-modal';
import { ApiDocumentService } from './services/document/api-document.service';
import { DOCUMENT_SERVICE } from './services/document/document-service.token';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideCharts(withDefaultRegisterables()),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([authInterceptor])),
    { provide: AUTH_SERVICE, useClass: JwtAuthService },
    { provide: USER_SERVICE, useClass: ApiUserService },
    { provide: WORKSPACE_SERVICE, useClass: ApiWorkspaceService },
    { provide: DOCUMENT_SERVICE, useClass: ApiDocumentService },
    importProvidersFrom(NgxSmartModalModule.forRoot())
  ],
};
