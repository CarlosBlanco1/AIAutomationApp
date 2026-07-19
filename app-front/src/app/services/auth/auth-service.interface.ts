import { WritableSignal } from "@angular/core";
import { Observable } from "rxjs";
import { LoginResponse } from "../../models/Auth/login-response";
import { CreateUserRequest } from "../../models/Users/create-user-request";
import { LoginRequest } from "../../models/Auth/login-request";

export interface AuthService
{
    register(request : CreateUserRequest) : Observable<string>;
    login(request : LoginRequest) : Observable<LoginResponse>;
    logout() : void;
    getToken() : string | null;
    isAuthenticated : WritableSignal<boolean>;
}