import { Inject, Injectable } from "@angular/core";
import { USER_SERVICE } from "../services/user/user-service.token";
import { UserService } from "../services/user/user-service.interface";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { map, Observable, of, pipe } from "rxjs";

@Injectable({ providedIn: 'root' })
export class EmailConfirmedGuard implements CanActivate {
  constructor(@Inject(USER_SERVICE) private userService: UserService, private router: Router) { }
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> {

    var user = this.userService.currentUser();

    if (user) {
      return of(Boolean(user.emailConfirmed));
    }

    return this.userService.fetchCurrentUser().pipe(map(user =>
      Boolean(user.emailConfirmed)
        ? true
        : this.router.createUrlTree(['/verify-email'])
    ))
  }
}