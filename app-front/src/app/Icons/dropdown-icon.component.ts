import { Component } from '@angular/core';

@Component({
  selector: 'app-dropdown-icon',
  template: `<svg
    xmlns="http://www.w3.org/2000/svg"
    width="24"
    height="24"
    viewBox="0 0 24 24"
    fill="none"
    stroke="#9CA3AF"
    stroke-width="2"
    stroke-linecap="round"
    stroke-linejoin="round"
    class="icon icon-tabler icons-tabler-outline icon-tabler-chevron-compact-down"
  >
    <path stroke="none" d="M0 0h24v24H0z" fill="none" />
    <path d="M4 11l8 3l8 -3" />
  </svg>`,
  standalone: true,
})
export class DropdownIconComponent {}
