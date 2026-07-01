import { Component, Inject, Input, signal } from '@angular/core';
import { DocumentIconComponent } from '../../icons/document-icon.component';
import { StarIconComponent } from '../../icons/start-icon.component';
import { DownloadIconComponent } from '../../icons/download-icon.component';
import { UsersIconComponent } from '../../icons/users-icon.component';
import { HorizontalDotsIconComponent } from '../../icons/horizontal-dots-icon.component';
import { SparkleIconComponent } from '../../icons/sparkle-icon.component';
import { RefreshIconComponent } from '../../icons/refresh-icon.component';
import { PointerRightIconComponent } from '../../icons/pointer-right-icon.component';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { ActivatedRoute } from '@angular/router';
import { DOCUMENT_SERVICE } from '../../services/document/document-service.token';
import { DocumentService } from '../../services/document/document-service.interface';
import { LoadingAnimationComponent } from "../../animations/loading-animation/loading-animation.component";
import { HttpClient } from '@angular/common/http';

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
    NgxExtendedPdfViewerModule,
    LoadingAnimationComponent
  ],
})

export class SingleDocumentComponent {
  constructor(private route: ActivatedRoute, @Inject(DOCUMENT_SERVICE) private documentService: DocumentService, private http: HttpClient) {
    route.paramMap.subscribe(params => {
      this.documentId = params.get('documentId')!;

      documentService.getDownloadUrl(this.documentId).subscribe({
        next: (res) => {
          this.pdfSrc.set(res.downloadUrl)

          this.http.get(this.pdfSrc(), { responseType: 'blob' }).subscribe({
            next: blob => {
              this.pdfBlobUrl.set(URL.createObjectURL(blob));
              console.log(this.pdfBlobUrl)
            },
            error: err => console.error(err)
          });
        }
      });
    })
  }

  documentId = '';

  pdfSrc = signal('');
  pdfBlobUrl = signal('')
}
