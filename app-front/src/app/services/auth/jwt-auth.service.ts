import { inject, Injectable, signal } from "@angular/core";
import { AuthService } from "./auth-service.interface";
import { Observable, tap } from "rxjs";
import { LoginRequest } from "../../models/Auth/login-request";
import { LoginResponse } from "../../models/Auth/login-response";
import { CreateUserRequest } from "../../models/Users/create-user-request";
import { HttpClient } from "@angular/common/http";
import { USER_SERVICE } from "../user/user-service.token";
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
        }));
    }

    logout(): void {
        localStorage.removeItem('token');
        this.isAuthenticated.set(false);
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }
}