import { map, Observable, tap } from "rxjs";
import { UserDto } from "../../models/Users/user-dto";
import { UserService } from "./user-service.interface";
import { inject, Injectable, signal, WritableSignal } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Injectable({providedIn : 'root'})
export class ApiUserService implements UserService {
    private readonly baseUrl = 'https://workspaceai.carlosblancodev.com/api/User';

    
    private httpClient = inject(HttpClient);
    
    currentUser: WritableSignal<UserDto | null> = signal(null);
    
    fetchCurrentUser(): Observable<void> {  
        return this.httpClient.get<UserDto>(`${this.baseUrl}/me`)
        .pipe(tap(res => this.currentUser.set(res)), map(() => void 0));
    }

    clearCurrentUser(): void {
        this.currentUser.set(null);
    }  
}