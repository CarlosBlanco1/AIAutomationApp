import { Component } from "@angular/core";
import { LogoComponent } from "../Logo/logo.component";
import { RouterLink } from "@angular/router";

@Component({
    selector: 'app-header',
    standalone: true,
    templateUrl : './header.component.html',
    styleUrl: './header.component.css',
    imports : [LogoComponent, RouterLink]
})

export class HeaderComponent {}