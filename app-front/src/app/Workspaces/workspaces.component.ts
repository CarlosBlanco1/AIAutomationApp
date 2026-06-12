import { Component } from '@angular/core';
import { SearchIconComponent } from '../Icons/search-icon.component';
import { GridIconComponent } from '../Icons/grid-icon.component';
import { BaselineIconComponent } from '../Icons/baseline-icon.component';
import { HouseIconComponent } from '../Icons/house-icon.component';
import { VerticalDotsIconComponent } from '../Icons/vertical-dots-icon.component';
import { StarIconComponent } from '../Icons/start-icon.component';
import { PlusIconComponent } from '../Icons/plus-icon.component';

@Component({
  selector: 'app-workspaces',
  templateUrl: './workspaces.component.html',
  standalone: true,
  imports: [
    SearchIconComponent,
    GridIconComponent,
    BaselineIconComponent,
    HouseIconComponent,
    VerticalDotsIconComponent,
    StarIconComponent,
    PlusIconComponent
  ],
})
export class WorkspacesComponent {}
