import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-loading-animation',
  imports: [],
  templateUrl: './loading-animation.component.html',
  styleUrl: './loading-animation.component.css'
})
export class LoadingAnimationComponent {
  @Input({ required: true }) widthAndHeight = '';
  @Input({ required: true }) spinnerPadding = '';
  @Input({ required: true }) borderStyle = '';
  @Input({ required: true }) spinnerColor = '';
}
