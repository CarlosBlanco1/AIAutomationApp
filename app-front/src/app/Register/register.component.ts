import { Component } from "@angular/core";
import { LogoComponent } from "../Logo/logo.component";
import { EmailIconComponent } from "../Icons/email-icon.component";
import { LockIconComponent } from "../Icons/lock-icon.component";
import { UserIconComponent } from "../Icons/user-icon.component";

@Component({
    selector : 'app-register',
    templateUrl : './register.component.html',
    standalone : true,
    imports : [LogoComponent, EmailIconComponent, LockIconComponent, UserIconComponent]
})

export class RegisterComponent{
    firstName = ""
    lastName = ""
    email = ""
    password = ""
}
