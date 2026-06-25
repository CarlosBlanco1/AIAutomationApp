import { Component, inject } from "@angular/core";
import { LogoComponent } from "../../logo/logo.component";
import { RouterLink } from "@angular/router";
import { AUTH_SERVICE } from "../../services/auth/auth-service.token";

@Component({
    selector: 'app-header',
    standalone: true,
    templateUrl : './header.component.html',
    styleUrl: './header.component.css',
    imports : [LogoComponent, RouterLink]
})

export class HeaderComponent {
    protected readonly authService = inject(AUTH_SERVICE);

    onSignOut() : void{
        this.authService.logout();
    }
}