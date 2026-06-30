import { Component, inject } from '@angular/core';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { DOCUMENT_SERVICE } from '../../../services/document/document-service.token';
import { TrashIconComponent } from "../../../icons/trash-icon.component";
import { LoadingAnimationComponent } from "../../../animations/loading-animation/loading-animation.component";
import { FailureCardComponent } from "../../state-cards/failure-card/failure-card.component";

@Component({
  selector: 'app-delete-document',
  imports: [TrashIconComponent, LoadingAnimationComponent, FailureCardComponent],
  templateUrl: './delete-document.component.html'
})
export class DeleteDocumentComponent {
  constructor(private modalService: NgxSmartModalService) {
    this.data = this.modalService.getModalData('deleteDocument') as { documentId: string, documentName : string }
  }

  data : { documentId: string, documentName : string } = { documentId : '', documentName : ''}
  documentService = inject(DOCUMENT_SERVICE)

  errorMessage = '';
  protected formState : DeleteFormState = 'form';

  onClickDelete() {
    var documentId = this.data.documentId;

    this.formState = 'loading';

    this.documentService.deleteDocument(documentId).subscribe({
      next : () => {
        this.onSuccess();
      },
      error : (err) => {
        this.formState = 'failure';
          if (err.error && typeof err.error === 'object') {
            this.errorMessage = err.error.message || 'An error occurred';
          } else {
            this.errorMessage = err.error
          }
      },
    })
  }

  onClickCancel() {
    this.modalService.get('deleteDocument').close();
  }

  onSuccess() {
    this.documentService.getUserDocuments().subscribe();
    this.modalService.get('deleteDocument').close();
  }

  tryAgain() {
    this.formState = 'form';
  }

}

type DeleteFormState = 'form' | 'loading' | 'failure';
