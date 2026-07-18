import { Inject, Injectable } from "@angular/core";
import { USER_SERVICE } from "../services/user/user-service.token";
import { UserService } from "../services/user/user-service.interface";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from "@angular/router";

@Injectable({ providedIn: 'root' })
export class EmailConfirmedGuard implements CanActivate {
  constructor(@Inject(USER_SERVICE) private userService: UserService, private router: Router) {}
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.userService.currentUser()?.emailConfirmed) {
      return true;
    }
    this.router.navigate(['/verify-email']);
    return false;
  }
}