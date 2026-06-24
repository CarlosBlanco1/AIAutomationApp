import { inject, Injectable } from "@angular/core";
import { AuthService } from "./auth-service.interface";
import { Observable, tap } from "rxjs";
import { LoginRequest } from "../../models/Auth/login-request";
import { LoginResponse } from "../../models/Auth/login-response";
import { CreateUserRequest } from "../../models/Users/create-user-request";
import { HttpClient } from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class JwtAuthService implements AuthService {
    private httpClient = inject(HttpClient);

    register(request: CreateUserRequest): Observable<string> {
        return this.httpClient.post('http://localhost:8080/api/Auth/Register',
            request,
            { responseType: 'text' });
    }

    login(request: LoginRequest): Observable<LoginResponse> {
        return this.httpClient.post<LoginResponse>('http://localhost:8080/api/Auth/Login',
            request
        ).pipe(tap(response => localStorage.setItem('token', response.jwtToken)));
    }

    logout(): void {
        localStorage.removeItem('token');
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }

    isAuthenticated(): boolean {
        return !!this.getToken();
    }
}