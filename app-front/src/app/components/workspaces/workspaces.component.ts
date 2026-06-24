import { Component } from '@angular/core';
import { SearchIconComponent } from '../../icons/search-icon.component';
import { GridIconComponent } from '../../icons/grid-icon.component';
import { BaselineIconComponent } from '../../icons/baseline-icon.component';
import { HouseIconComponent } from '../../icons/house-icon.component';
import { VerticalDotsIconComponent } from '../../icons/vertical-dots-icon.component';
import { StarIconComponent } from '../../icons/start-icon.component';
import { PlusIconComponent } from '../../icons/plus-icon.component';

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
