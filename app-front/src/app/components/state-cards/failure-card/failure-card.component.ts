import { Component, Input } from '@angular/core';
import { FailureIconComponent } from '../../../icons/failure-icon.component';
import { LockIconComponent } from '../../../icons/lock-icon.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-failure-card',
  imports: [FailureIconComponent, LockIconComponent, RouterLink],
  templateUrl: './failure-card.component.html',
})
export class FailureCardComponent {
  @Input({ required : true}) primaryText! : string;
  @Input({ required : true}) errorMessage! : string;
  @Input({ required : true}) tryAgainMethod! : () => void;
  @Input({ required : true}) secondaryText! : string;
  @Input({ required : true}) secondaryUrl! : string;
}
