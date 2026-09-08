import { Component, input } from "@angular/core";
import { ClockIconComponent } from "../../../icons/clock-icon.component";
import { LoadingAnimationComponent } from "../../../animations/loading-animation/loading-animation.component";
import { AlertIconComponent } from "../../../icons/alert-icon.component";
import { CheckIconComponent } from "../../../icons/check-icon.component";

@Component({
    selector : 'app-status-pill',
    templateUrl : './status-pill.component.html',
    imports: [ClockIconComponent, LoadingAnimationComponent, AlertIconComponent, CheckIconComponent]
})

export class StatsuPillComponent {
    processingStatus = input.required<string>();   
}