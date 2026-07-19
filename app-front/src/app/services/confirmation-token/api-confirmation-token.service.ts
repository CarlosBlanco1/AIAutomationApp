import { Observable, tap } from "rxjs";
import { ConfirmationTokenService } from "./confirmation-token-service.interface";
import { inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { USER_SERVICE } from "../user/user-service.token";

@Injectable({ providedIn: 'root' })
export class ApiConfirmationTokenService implements ConfirmationTokenService {
    private readonly baseUrl = 'https://workspaceai.carlosblancodev.com/api/ConfirmationToken';

    private readonly httpClient = inject(HttpClient);
    private readonly userService = inject(USER_SERVICE);

    generateConfirmationToken(): Observable<string> {
        return this.httpClient.post<string>(`${this.baseUrl}/email/generate`, {})
    }

    validateConfirmationToken(userId: string, token: string): Observable<string> {
        return this.httpClient.get(`${this.baseUrl}/email?userId=${userId}&token=${token}`, { responseType: 'text' })
    }
}