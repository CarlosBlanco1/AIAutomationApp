import { Component } from '@angular/core';
import { DocumentIconComponent } from '../../icons/document-icon.component';
import { StarIconComponent } from '../../icons/start-icon.component';
import { DownloadIconComponent } from '../../icons/download-icon.component';
import { UsersIconComponent } from '../../icons/users-icon.component';
import { HorizontalDotsIconComponent } from '../../icons/horizontal-dots-icon.component';
import { SparkleIconComponent } from '../../icons/sparkle-icon.component';
import { RefreshIconComponent } from '../../icons/refresh-icon.component';
import { PointerRightIconComponent } from '../../icons/pointer-right-icon.component';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';

@Component({
  selector: 'app-single-document',
  templateUrl: './single-document.component.html',
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
