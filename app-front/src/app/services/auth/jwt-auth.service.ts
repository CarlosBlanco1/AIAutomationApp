import { inject, Injectable, signal } from "@angular/core";
import { AuthService } from "./auth-service.interface";
import { Observable, tap } from "rxjs";
import { LoginRequest } from "../../models/Auth/login-request";
import { LoginResponse } from "../../models/Auth/login-response";
import { CreateUserRequest } from "../../models/Users/create-user-request";
import { HttpClient } from "@angular/common/http";
import { USER_SERVICE } from "../user/user-service.token";

@Injectable({ providedIn: 'root' })
export class JwtAuthService implements AuthService {
    private readonly baseUrl = 'https://workspaceai.carlosblancodev.com/api/Auth';

    private httpClient = inject(HttpClient);
    private userService = inject(USER_SERVICE);

    isAuthenticated = signal<boolean>(!!this.getToken())

    register(request: CreateUserRequest): Observable<string> {
        return this.httpClient.post(`${this.baseUrl}/Register`,
            request,
            { responseType: 'text' });
    }

    login(request: LoginRequest): Observable<LoginResponse> {
        return this.httpClient.post<LoginResponse>(`${this.baseUrl}/Login`,
            request
        ).pipe(tap(response => {
            localStorage.setItem('token', response.jwtToken);
            this.isAuthenticated.set(true);
        }));
    }

    logout(): void {
        localStorage.removeItem('token');
        this.isAuthenticated.set(false);
        this.userService.clearCurrentUser();
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }
}