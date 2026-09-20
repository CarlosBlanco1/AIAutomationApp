import { inject, Injectable, signal } from "@angular/core";
import { AuthService } from "./auth-service.interface";
import { map, Observable, tap } from "rxjs";
import { LoginRequest } from "../../models/Auth/login-request";
import { LoginResponse } from "../../models/Auth/login-response";
import { CreateUserRequest } from "../../models/Users/create-user-request";
import { HttpClient } from "@angular/common/http";
import { AppConfigService } from "../configuration/app-config.service";

@Injectable({ providedIn: 'root' })
export class JwtAuthService implements AuthService {

    private readonly httpClient = inject(HttpClient);
    private readonly configService = inject(AppConfigService);

    isAuthenticated = signal<boolean>(!!this.getToken())

    register(request: CreateUserRequest): Observable<string> {
        var baseUrl = `${this.configService.apiUrl}/api/Auth`;

        return this.httpClient.post(`${baseUrl}/Register`,
            request,
            { responseType: 'text' });
    }

    login(request: LoginRequest): Observable<LoginResponse> {
        var baseUrl = `${this.configService.apiUrl}/api/Auth`;

        return this.httpClient.post<LoginResponse>(`${baseUrl}/Login`,
            request
        ).pipe(tap(response => {
            localStorage.setItem('token', response.jwtToken);
            this.isAuthenticated.set(true);
        }));
    }

    logout(): Observable<void> {
        var baseUrl = `${this.configService.apiUrl}/api/Auth`;

        return this.httpClient.post<void>(`${baseUrl}/Logout`, {}, { withCredentials: true }).pipe(
            tap(() => {
                localStorage.removeItem('token');
                this.isAuthenticated.set(false);
            })
        )
    }

    fetchNewAcessToken(): Observable<void> {
        var baseUrl = `${this.configService.apiUrl}/api/Auth`;

        return this.httpClient.post<{ jwtToken: string }>(`${baseUrl}/Refresh`, {}, { withCredentials: true }).pipe(
            tap((res) => {
                localStorage.setItem('token', res.jwtToken)
                this.isAuthenticated.set(true);
            }),
            map(() => void 0)
        )
    }

    clearLocalSession(): void {
        localStorage.removeItem('token')
        this.isAuthenticated.set(false)
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }
}