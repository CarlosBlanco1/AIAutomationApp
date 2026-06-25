import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

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

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    { provide: AUTH_SERVICE, useClass: JwtAuthService },
    { provide: USER_SERVICE, useClass: ApiUserService },
    { provide: WORKSPACE_SERVICE, useClass: ApiWorkspaceService },
    importProvidersFrom(NgxSmartModalModule.forRoot())
  ],
};
