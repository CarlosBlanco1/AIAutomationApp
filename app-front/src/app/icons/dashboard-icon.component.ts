import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-dashboard-icon',
    template: `<svg xmlns="http://www.w3.org/2000/svg"
    viewBox="0 0 24 24" 
    fill="none" 
    stroke="currentColor" 
    stroke-width="2" 
    stroke-linecap="round" 
    stroke-linejoin="round" 
    [class]="'icon icon-tabler icons-tabler-outline icon-tabler-chart-line ' + svgClass">
	<path stroke="none" d="M0 0h24v24H0z" fill="none" />
	<path d="M4 19l16 0" />
	<path d="M4 15l4 -6l4 2l4 -5l4 4" />
</svg>`,
    standalone: true
})

export class DashboardIconComponent {
    @Input() svgClass = '';
}
