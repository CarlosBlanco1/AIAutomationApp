import { Component } from '@angular/core';
import { DocumentMetricBlockComponent } from './document-metric-block.component';
import { DocumentTableRowComponent } from './document-table-row.component';
import { BaselineIconComponent } from '../../icons/baseline-icon.component';
import { GridIconComponent } from '../../icons/grid-icon.component';
import { HouseIconComponent } from '../../icons/house-icon.component';
import { LeftArrowIcon } from '../../icons/left-arrow-icon.component';
import { PlusIconComponent } from '../../icons/plus-icon.component';
import { RightArrowIcon } from '../../icons/right-arrow-icon.component';
import { SearchIconComponent } from '../../icons/search-icon.component';

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
