import { Component, inject, ViewContainerRef } from '@angular/core';
import { DocumentMetricBlockComponent } from './document-metric-block.component';
import { DocumentTableRowComponent } from './document-table-row.component';
import { BaselineIconComponent } from '../../icons/baseline-icon.component';
import { GridIconComponent } from '../../icons/grid-icon.component';
import { HouseIconComponent } from '../../icons/house-icon.component';
import { LeftArrowIcon } from '../../icons/left-arrow-icon.component';
import { PlusIconComponent } from '../../icons/plus-icon.component';
import { RightArrowIcon } from '../../icons/right-arrow-icon.component';
import { SearchIconComponent } from '../../icons/search-icon.component';
import { DOCUMENT_SERVICE } from '../../services/document/document-service.token';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { CreateDocumentComponent } from './create-document/create-document.component';
import { WORKSPACE_SERVICE } from '../../services/workspace/workspace-service.token';

@Component({
  selector: 'app-documents',
  templateUrl: './documents.component.html',
  standalone: true,
  imports: [
    HouseIconComponent,
    SearchIconComponent,
    PlusIconComponent,
    LeftArrowIcon,
    RightArrowIcon,
    BaselineIconComponent,
    GridIconComponent,
    DocumentMetricBlockComponent,
    DocumentTableRowComponent
  ],
})
export class DocumentsComponent {
  documentService = inject(DOCUMENT_SERVICE)
  workspaceService = inject(WORKSPACE_SERVICE)

  constructor(private ngxSmartModalService: NgxSmartModalService, private vcr: ViewContainerRef) {
    this.documentService.getUserDocuments().subscribe()
    this.workspaceService.getUserWorkspaces().subscribe()
  }

  onAddDocument() {
    if(this.workspaceService.userWorkspaces().length < 1)
    {
      window.alert("You must have at least 1 workspace to upload a new document")
    }
    else
    {
      this.ngxSmartModalService.create('createDocument', CreateDocumentComponent, this.vcr).open();
    }
  }
}
