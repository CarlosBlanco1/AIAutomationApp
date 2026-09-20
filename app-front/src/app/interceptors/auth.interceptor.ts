import { HttpEvent, HttpHandlerFn, HttpRequest } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, Observable, switchMap, throwError } from "rxjs";
import { AUTH_SERVICE } from "../services/auth/auth-service.token";

export function authInterceptor(
    req: HttpRequest<unknown>,
    next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> {
    const authService = inject(AUTH_SERVICE);

    const token = localStorage.getItem('token');

    const isR2Request = req.url.includes('.r2.cloudflarestorage.com');
    const isRefreshRequest = req.url.includes('/api/Auth/Refresh')

    if (token && !isR2Request && !isRefreshRequest) {
        req = req.clone({
            headers: req.headers.set('Authorization', `Bearer ${token}`),
        });
    }

    return next(req).pipe(
        catchError(err => {
            if (err.status !== 401 || isRefreshRequest || isR2Request) {
                return throwError(() => err);
            }

            return authService.fetchNewAcessToken().pipe(
                switchMap(() => {
                    const newJwt = localStorage.getItem('token')

                    req = req.clone({
                        headers: req.headers.set('Authorization', `Bearer ${newJwt}`),
                    });

                    return next(req)
                })
                ,
                catchError(refreshError => {
                    authService.clearLocalSession();
                    return throwError(() => refreshError);
                })
            )
        })
    )
}