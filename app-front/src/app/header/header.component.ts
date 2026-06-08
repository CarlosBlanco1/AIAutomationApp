import { Component } from "@angular/core";
import { SparkleIconComponent } from "../Icons/sparkle-icon.component";

@Component({
    selector: 'app-header',
    standalone: true,
    templateUrl : './header.component.html',
    styleUrl: './header.component.css',
    imports : [SparkleIconComponent]
})

export class HeaderComponent {}