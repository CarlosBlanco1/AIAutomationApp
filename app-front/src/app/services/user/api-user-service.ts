import { catchError, Observable, of, shareReplay, tap, throwError } from "rxjs";
import { UserDto } from "../../models/Users/user-dto";
import { UserService } from "./user-service.interface";
import { inject, Injectable, signal, WritableSignal } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { AppConfigService } from "../configuration/app-config.service";
import { AUTH_SERVICE } from "../auth/auth-service.token";

@Injectable({providedIn : 'root'})
export class ApiUserService implements UserService {
    private httpClient = inject(HttpClient);
    private configService = inject(AppConfigService);
    private authService = inject(AUTH_SERVICE)
    
    currentUser: WritableSignal<UserDto | null> = signal(null);
    
    fetchCurrentUser(): Observable<UserDto | null> {  
        var baseUrl = `${this.configService.apiUrl}/api/User`;

        return this.httpClient.get<UserDto | null>(`${baseUrl}/me`)
        .pipe(tap(res => this.currentUser.set(res)), 
        catchError(error => {
            if(error.status == 401)
            {
                this.authService.logout();
                this.clearCurrentUser();
                return of(null);
            }

            return throwError(() => error);
        }));
    }

    clearCurrentUser(): void {
        this.currentUser.set(null);
    }  
}