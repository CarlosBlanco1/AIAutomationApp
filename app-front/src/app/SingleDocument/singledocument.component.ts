import { Component } from '@angular/core';
import { DocumentIconComponent } from '../Icons/document-icon.component';
import { StarIconComponent } from '../Icons/start-icon.component';
import { DownloadIconComponent } from '../Icons/download-icon.component';
import { UsersIconComponent } from '../Icons/users-icon.component';
import { HorizontalDotsIconComponent } from '../Icons/horizontal-dots-icon.component';
import { SparkleIconComponent } from '../Icons/sparkle-icon.component';
import { RefreshIconComponent } from '../Icons/refresh-icon.component';
import { PointerRightIconComponent } from '../Icons/pointer-right-icon.component';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';

@Component({
  selector: 'app-single-document',
  templateUrl: './singledocument.component.html',
  standalone: true,
  imports: [
    DocumentIconComponent,
    StarIconComponent,
    DownloadIconComponent,
    UsersIconComponent,
    HorizontalDotsIconComponent,
    SparkleIconComponent,
    RefreshIconComponent,
    PointerRightIconComponent,
    NgxExtendedPdfViewerModule
  ],
})
export class SingleDocumentComponent {
  pdfSrc = "https://vadimdez.github.io/ng2-pdf-viewer/assets/pdf-test.pdf";
}
