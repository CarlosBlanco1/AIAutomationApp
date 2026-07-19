import { Observable, tap } from "rxjs";
import { ConfirmationTokenService } from "./confirmation-token-service.interface";
import { inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { USER_SERVICE } from "../user/user-service.token";
import { AUTH_SERVICE } from "../auth/auth-service.token";

@Injectable({ providedIn: 'root' })
export class ApiConfirmationTokenService implements ConfirmationTokenService {
    private readonly baseUrl = 'https://workspaceai.carlosblancodev.com/api/ConfirmationToken';

    private readonly httpClient = inject(HttpClient);
    private readonly authService = inject(AUTH_SERVICE);

    generateConfirmationToken(): Observable<string> {
        return this.httpClient.post<string>(`${this.baseUrl}/email/generate`, {})
    }

    validateConfirmationToken(userId: string, token: string): Observable<{jwtToken : string}> {
        return this.httpClient.get<{jwtToken : string}>(`${this.baseUrl}/email?userId=${userId}&token=${token}`)
        .pipe(tap(response => {
            localStorage.setItem('token', response.jwtToken);
            this.authService.isAuthenticated.set(true);
        }))
    }
}