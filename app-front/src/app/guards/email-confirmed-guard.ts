import { Inject, Injectable } from "@angular/core";
import { USER_SERVICE } from "../services/user/user-service.token";
import { UserService } from "../services/user/user-service.interface";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { map, Observable, of } from "rxjs";

@Injectable({ providedIn: 'root' })
export class EmailConfirmedGuard implements CanActivate {
  constructor(@Inject(USER_SERVICE) private userService: UserService, private router: Router) { }
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> {

    var user = this.userService.currentUser();

    if (user) {
      console.log("CURRENT USER WASNT NULL!")
      console.log(user)

      return Boolean(user.emailConfirmed)
        ? of(true)
        : of(this.router.createUrlTree(['/verify-email']));
    }

    return this.userService.fetchCurrentUser().pipe(map(user =>
    {
      console.log("USER WAS NULL, REFETCHING");
      console.log(user);
      
      if(!user)
      {
        return this.router.createUrlTree(['/login']);
      }

      return Boolean(user.emailConfirmed)
        ? true
        : this.router.createUrlTree(['/verify-email']);
    }
  ))
  }
}