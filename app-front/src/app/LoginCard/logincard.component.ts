import { Component } from "@angular/core";
import { EmailIconComponent } from "../Icons/email-icon.component";
import { LockIconComponent } from "../Icons/lock-icon.component";
import { ArrowIconComponent } from "../Icons/arrow-icon.component";

@Component({
    selector : 'app-login',
    standalone : true,
    templateUrl : './logincard.component.html',
    styleUrl : './logincard.component.css',
    imports : [EmailIconComponent, LockIconComponent, ArrowIconComponent]
})

export class LoginCardComponent {
    email = ""
    password = ""
}