import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { LoginResponse } from "../../Models/Auth/login-response";
import { CreateUserRequest } from "../../Models/Users/create-user-request";
import { LoginRequest } from "../../Models/Auth/login-request";

export interface AuthService
{
    register(request : CreateUserRequest) : Observable<void>;
    login(request : LoginRequest) : Observable<LoginResponse>;
}