import { Observable } from "rxjs";

export interface ConfirmationTokenService
{
    generateConfirmationToken() : Observable<string>;
    validateConfirmationToken(userId : string, token : string) : Observable<string>;
}