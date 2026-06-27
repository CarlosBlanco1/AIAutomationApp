import { Component, inject, ViewContainerRef } from '@angular/core';
import { SearchIconComponent } from '../../icons/search-icon.component';
import { GridIconComponent } from '../../icons/grid-icon.component';
import { BaselineIconComponent } from '../../icons/baseline-icon.component';
import { HouseIconComponent } from '../../icons/house-icon.component';
import { VerticalDotsIconComponent } from '../../icons/vertical-dots-icon.component';
import { StarIconComponent } from '../../icons/start-icon.component';
import { PlusIconComponent } from '../../icons/plus-icon.component';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { CreateWorkspaceComponent } from './create-workspace/create-workspace.component';
import { WORKSPACE_SERVICE } from '../../services/workspace/workspace-service.token';
import { UsersIconComponent } from "../../icons/users-icon.component";
import { DocumentIconComponent } from "../../icons/document-icon.component";
import { DatabaseIconComponent } from "../../icons/database-icon.component";

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
    PlusIconComponent,
    UsersIconComponent,
    DocumentIconComponent,
    DatabaseIconComponent
],
})
export class WorkspacesComponent {

  workspaceService = inject(WORKSPACE_SERVICE);

  constructor(private ngxSmartModalService: NgxSmartModalService, private vcr: ViewContainerRef) {
    this.workspaceService.getUserWorkspaces().subscribe();
  }

  onOpenModal() {
    this.ngxSmartModalService.create('createWorkspace', CreateWorkspaceComponent, this.vcr).open();
  }

}
