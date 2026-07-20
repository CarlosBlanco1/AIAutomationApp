import { map, Observable, tap } from "rxjs";
import { UserDto } from "../../models/Users/user-dto";
import { UserService } from "./user-service.interface";
import { inject, Injectable, signal, WritableSignal } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { AppConfigService } from "../configuration/app-config.service";

@Injectable({providedIn : 'root'})
export class ApiUserService implements UserService {
    private baseUrl? : string;
    private httpClient = inject(HttpClient);
    private configService = inject(AppConfigService);
    
    constructor() {
        this.baseUrl = `${this.configService.apiUrl}/api/User`;
    }
    
    currentUser: WritableSignal<UserDto | null> = signal(null);
    
    fetchCurrentUser(): Observable<void> {  
        return this.httpClient.get<UserDto>(`${this.baseUrl}/me`)
        .pipe(tap(res => this.currentUser.set(res)), map(() => void 0));
    }

    clearCurrentUser(): void {
        this.currentUser.set(null);
    }  
}