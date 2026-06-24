import { Inject, Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from "@angular/router";
import { AuthService } from "../services/auth/auth-service.interface";
import { AUTH_SERVICE } from "../services/auth/auth-service.token";

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(@Inject(AUTH_SERVICE) private authService: AuthService, private router: Router) {}
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.authService.isAuthenticated()) {
      return true;
    }
    this.router.navigate(['/login']);
    return false;
  }
}