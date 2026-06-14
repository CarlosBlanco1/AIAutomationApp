import { Component } from '@angular/core';
import { HorizontalDotsIconComponent } from '../Icons/horizontal-dots-icon.component';
import { HouseIconComponent } from '../Icons/house-icon.component';
import { SearchIconComponent } from '../Icons/search-icon.component';
import { PlusIconComponent } from '../Icons/plus-icon.component';
import { LeftArrowIcon } from '../Icons/left-arrow-icon.component';
import { RightArrowIcon } from '../Icons/right-arrow-icon.component';
import { BaselineIconComponent } from '../Icons/baseline-icon.component';
import { GridIconComponent } from '../Icons/grid-icon.component';
import { DocumentMetricBlockComponent } from './document-metric-block.component';
import { DocumentTableRowComponent } from './document-table-row.component';

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
export class DocumentsComponent {}
