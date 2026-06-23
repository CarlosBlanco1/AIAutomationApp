import { Component, Input } from '@angular/core';
import { FailureIconComponent } from '../../Icons/failure-icon.component';
import { LockIconComponent } from '../../Icons/lock-icon.component';

@Component({
  selector: 'app-failure-card',
  imports: [FailureIconComponent, LockIconComponent],
  templateUrl: './failure-card.component.html',
})
export class FailureCardComponent {
  @Input({ required : true}) primaryText! : string;
  @Input({ required : true}) errorMessage! : string;
  @Input({ required : true}) tryAgainMethod! : () => void;
  @Input({ required : true}) secondaryText! : string;
  @Input({ required : true}) secondaryUrl! : string;
}
