import { inject, Injectable } from "@angular/core";
import { AuthService } from "./auth-service.interface";
import { Observable } from "rxjs";
import { LoginRequest } from "../../Models/Auth/login-request";
import { LoginResponse } from "../../Models/Auth/login-response";
import { CreateUserRequest } from "../../Models/Users/create-user-request";
import { HttpClient } from "@angular/common/http";

@Injectable({providedIn : 'root'})
export class JwtAuthService implements AuthService{
    private httpClient = inject(HttpClient);

    register(request: CreateUserRequest): Observable<void> {
        return this.httpClient.post<void>('/api/Auth/Register',
            request
        );
    }
    
    login(request: LoginRequest): Observable<LoginResponse> {
        return this.httpClient.post<LoginResponse>('/api/Auth/Login',
            request
        );
    }

}