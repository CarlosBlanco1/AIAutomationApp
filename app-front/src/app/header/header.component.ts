import { Component } from "@angular/core";
import { LogoComponent } from "../Logo/logo.component";

@Component({
    selector: 'app-header',
    standalone: true,
    templateUrl : './header.component.html',
    styleUrl: './header.component.css',
    imports : [LogoComponent]
})

export class HeaderComponent {}