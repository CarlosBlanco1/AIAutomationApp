import { Component, inject } from "@angular/core";
import { USER_SERVICE } from "../../services/user/user-service.token";

@Component({
    templateUrl : './email-confirmed.component.html'
})
export class EmailConfirmedComponent{
    protected readonly userService = inject(USER_SERVICE)
}