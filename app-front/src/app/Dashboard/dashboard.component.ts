import { Component } from "@angular/core";
import { DocumentIconComponent } from "../Icons/document-icon.component";
import { EyeIconComponent } from "../Icons/eye-icon.component";
import { VerticalDotsIconComponent } from "../Icons/vertical-dots-icon.component";
import { PointerRightIconComponent } from "../Icons/pointer-right-icon.component";

@Component({
    selector : 'app-dashboard',
    templateUrl : './dashboard.component.html',
    standalone : true,
    imports : [DocumentIconComponent, EyeIconComponent, VerticalDotsIconComponent, PointerRightIconComponent]
})

export class DashboardComponent {}