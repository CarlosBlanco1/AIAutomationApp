import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-sparkline',
  standalone: true,
  template: `
    <svg
      [attr.width]="size"
      [attr.height]="size / 2"
      viewBox="0 0 240 120"
      role="img"
      aria-label="Upward trend chart"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M12 25H228 M12 55H228 M12 85H228 M12 110H228"
        fill="none"
        stroke="currentColor"
        stroke-opacity=".08"
      />

      <path
        d="M12 84 L42 72 L70 78 L100 48 L130 58 L158 35 L188 43 L216 18 L228 22 L228 110 L12 110 Z"
        [attr.fill]="color"
        fill-opacity=".14"
      />

      <path
        d="M12 84 L42 72 L70 78 L100 48 L130 58 L158 35 L188 43 L216 18 L228 22"
        fill="none"
        [attr.stroke]="color"
        stroke-width="3"
        stroke-linecap="round"
        stroke-linejoin="round"
      />

      <circle cx="228" cy="22" r="4" [attr.fill]="color" />
    </svg>
  `,
})
export class SparklineComponent {
  @Input() size = 240;
  @Input() color = '#38bdf8';
}