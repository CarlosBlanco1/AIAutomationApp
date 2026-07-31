import { Component, Inject, signal } from '@angular/core';
import { DocumentIconComponent } from '../../icons/document-icon.component';
import { StarIconComponent } from '../../icons/start-icon.component';
import { DownloadIconComponent } from '../../icons/download-icon.component';
import { UsersIconComponent } from '../../icons/users-icon.component';
import { HorizontalDotsIconComponent } from '../../icons/horizontal-dots-icon.component';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { ActivatedRoute } from '@angular/router';
import { DOCUMENT_SERVICE } from '../../services/document/document-service.token';
import { DocumentService } from '../../services/document/document-service.interface';
import { LoadingAnimationComponent } from "../../animations/loading-animation/loading-animation.component";
import { HttpClient } from '@angular/common/http';
import { DocumentDto } from '../../models/Documents/document-dto';
import { FolderIconComponent } from "../../icons/folder-icon.component";
import { CalendarIconComponent } from "../../icons/calendar-icon.component";
import { AlphabetIconComponent } from "../../icons/alphabet-icon.component";
import { SummaryIconComponent } from "../../icons/summary-icon.component";
import { AiChatComponent } from "../ai-chat/ai-chat.component";

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
    NgxExtendedPdfViewerModule,
    LoadingAnimationComponent,
    FolderIconComponent,
    CalendarIconComponent,
    AlphabetIconComponent,
    SummaryIconComponent,
    AiChatComponent
],
})

export class SingleDocumentComponent {
  constructor(private route: ActivatedRoute, @Inject(DOCUMENT_SERVICE) private documentService: DocumentService, private http: HttpClient) {
    route.paramMap.subscribe(params => {
     this.documentId.set(params.get('documentId')!);

      if(this.documentId() == null)
      {
        throw new Error("No document Id was passed in!");
      }

      documentService.getSingleDocument(this.documentId()!).subscribe({
        next: (res) => {
          this.document.set(res);
        },
        error: (err) => {
          if (err.error && typeof err.error === 'object') {
            this.errorMessage = err.error.message || 'An error occurred';
          } else {
            this.errorMessage = err.error
          }
        }
      })

      documentService.getDownloadUrl(this.documentId()!).subscribe({
        next: (res) => {
          this.pdfUrl.set(res.downloadUrl);

          this.http.get(this.pdfUrl()!, { responseType: 'blob' }).subscribe({
            next: blob => {
              this.pdfBlobUrl.set(URL.createObjectURL(blob));
            },
            error: (err) => {
              if (err.error && typeof err.error === 'object') {
                this.errorMessage = err.error.message || 'An error occurred';
              } else {
                this.errorMessage = err.error
              }
            }
          });
        }
      });
    })
  }

  documentId = signal<string | null>(null);
  pdfUrl = signal<string | null>(null);
  pdfBlobUrl = signal<string | null>(null);
  document = signal<DocumentDto | null>(null);
  errorMessage: string | null = null;

  getDate(): string {
    var date = new Date(this.document()!.createdAt)

    var dayAndMonth = date.toDateString().match(/^.{4}(.*)$/)![1]
    var hourAndMinute = date.toLocaleTimeString("en-US", {
      hour: 'numeric',
      minute: '2-digit',
      hour12: true
    })

    return `${dayAndMonth.slice(0, 6) + ',' + dayAndMonth.slice(6)} * ${hourAndMinute}`
  }
}