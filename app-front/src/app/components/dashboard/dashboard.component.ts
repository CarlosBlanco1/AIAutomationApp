import { Component } from "@angular/core";
import { DocumentIconComponent } from "../../icons/document-icon.component";
import { EyeIconComponent } from "../../icons/eye-icon.component";
import { PointerRightIconComponent } from "../../icons/pointer-right-icon.component";
import { VerticalDotsIconComponent } from "../../icons/vertical-dots-icon.component";


@Component({
    selector : 'app-dashboard',
    templateUrl : './dashboard.component.html',
    standalone : true,
    imports : [DocumentIconComponent, EyeIconComponent, VerticalDotsIconComponent, PointerRightIconComponent]
})

export class DashboardComponent {}