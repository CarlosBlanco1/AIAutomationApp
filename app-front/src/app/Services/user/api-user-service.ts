import { map, Observable, tap } from "rxjs";
import { UserDto } from "../../models/Users/user-dto";
import { UserService } from "./user-service.interface";
import { inject, Injectable, signal, Signal, WritableSignal } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Injectable({providedIn : 'root'})
export class ApiUserService implements UserService {
    
    private httpClient = inject(HttpClient);
    
    currentUser: WritableSignal<UserDto | null> = signal(null);
    
    fetchCurrentUser(): Observable<void> {  
        return this.httpClient.get<UserDto>('http://localhost:8080/api/User/me')
        .pipe(tap(res => this.currentUser.set(res)), map(() => void 0));
    }

    clearCurrentUser(): void {
        this.currentUser.set(null);
    }
    
}