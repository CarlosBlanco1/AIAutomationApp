import { Component, inject } from "@angular/core";
import { USER_SERVICE } from "../../services/user/user-service.token";
import { ClockIconComponent } from "../../icons/clock-icon.component";
import { RefreshIconComponent } from "../../icons/refresh-icon.component";
import { SendIconComponent } from "../../icons/send-icon.component";
import { RouterLink } from "@angular/router";

@Component({
    selector : 'app-email-confirmed',
    templateUrl : './email-confirmed.component.html',
    imports: [ClockIconComponent, RefreshIconComponent, SendIconComponent, RouterLink]
})
export class EmailConfirmedComponent{
    protected readonly userService = inject(USER_SERVICE)
}