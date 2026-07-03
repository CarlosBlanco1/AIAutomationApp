import { Component, inject, signal } from "@angular/core";
import { DocumentIconComponent } from "../../icons/document-icon.component";
import { EyeIconComponent } from "../../icons/eye-icon.component";
import { PointerRightIconComponent } from "../../icons/pointer-right-icon.component";
import { VerticalDotsIconComponent } from "../../icons/vertical-dots-icon.component";
import { USER_SERVICE } from "../../services/user/user-service.token";
import { DOCUMENT_SERVICE } from "../../services/document/document-service.token";


@Component({
    selector : 'app-dashboard',
    templateUrl : './dashboard.component.html',
    standalone : true,
    imports : [DocumentIconComponent, EyeIconComponent, VerticalDotsIconComponent, PointerRightIconComponent]
})

export class DashboardComponent {

    userService = inject(USER_SERVICE)
    documentService = inject(DOCUMENT_SERVICE)

    constructor() {
        this.documentService.getUserDocuments().subscribe();
    }

}