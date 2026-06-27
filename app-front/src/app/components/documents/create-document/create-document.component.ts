import { Component, inject } from '@angular/core';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { DOCUMENT_SERVICE } from '../../../services/document/document-service.token';
import { getRuleToMessageText } from '../../../dictionaries/validation-messages';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SparkleIconComponent } from "../../../icons/sparkle-icon.component";
import { InputValidatorComponent } from "../../register/input-validator/input-validator.component";
import { LoadingAnimationComponent } from "../../../animations/loading-animation/loading-animation.component";
import { FailureCardComponent } from "../../state-cards/failure-card/failure-card.component";

@Component({
  selector: 'app-create-document',
  imports: [SparkleIconComponent, ReactiveFormsModule, InputValidatorComponent, LoadingAnimationComponent, FailureCardComponent],
  templateUrl: './create-document.component.html'
})
export class CreateDocumentComponent {
constructor(private modalService: NgxSmartModalService) { }

  onCancel() {
    this.modalService.get('createDocument').close();
  }

  onSuccess() {
    this.documentService.getUserDocuments().subscribe();
    this.modalService.get('createDocument').close();
  }

  private documentService = inject(DOCUMENT_SERVICE)

  protected formState: createDocumentFormState = 'form';

  errorMessage = ''
  textValidationMessages = getRuleToMessageText('Document Name', 2, 50);

  protected documentForm = new FormGroup(
    {
      documentName: new FormControl('', [
        Validators.required,
        Validators.maxLength(50),
        Validators.minLength(2)
      ])
    }
  )

  get documentName() {
    return this.documentForm.get('documentName') as FormControl<string | null>
  }

  onSubmitForm() {
    if (this.documentForm.invalid) {
      return;
    }

    this.formState = 'loading';

    this.documentService.createDocument({
      workspaceId: '',
      fileName: '',
      filePath: '',
      fileText: '',
      summary: ''
    }).subscribe(
      {
        next: () => {
          this.onSuccess()
        },
        error: (err) => {
          this.formState = 'failure';
          if (err.error && typeof err.error === 'object') {
            this.errorMessage = err.error.message || 'An error occurred';
          } else {
            this.errorMessage = err.error
          }
        }
      }
    )
  }

  tryAgain() {
    this.documentForm.reset();
    this.documentForm.markAsPristine();
    this.documentForm.markAsUntouched();
    this.formState = 'form';
  }
}

type createDocumentFormState = 'form' | 'failure' | 'loading'

