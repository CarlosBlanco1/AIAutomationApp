import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { JwtAuthService } from './Services/Auth/jwt-auth.service';
import { AUTH_SERVICE } from './Services/Auth/auth-service.token';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    { provide: AUTH_SERVICE, useClass: JwtAuthService },
    provideRouter(routes)
  ],
};
