import { Observable } from "rxjs";
import { ConfirmationTokenService } from "./confirmation-token-service.interface";
import { inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class ApiConfirmationTokenService implements ConfirmationTokenService {
    private readonly baseUrl = 'https://localhost:8080/api/ConfirmationToken';
    
    private readonly httpClient = inject(HttpClient)
    
    generateConfirmationToken(): Observable<string> {
        return this.httpClient.post<string>(`${this.baseUrl}/email/generate`, {})
    }

    validateConfirmationToken(userId: string, token: string): Observable<string> {
        return this.httpClient.get<string>(`${this.baseUrl}/email?userId=${userId}&token=${token}`)
    }
}