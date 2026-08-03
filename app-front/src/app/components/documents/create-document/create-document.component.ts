import { Component, inject } from '@angular/core';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { DOCUMENT_SERVICE } from '../../../services/document/document-service.token';
import { getRuleToMessageFile, getRuleToMessageText } from '../../../dictionaries/validation-messages';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputValidatorComponent } from "../../register/input-validator/input-validator.component";
import { LoadingAnimationComponent } from "../../../animations/loading-animation/loading-animation.component";
import { FailureCardComponent } from "../../state-cards/failure-card/failure-card.component";
import { fileValidator } from '../../../directives/Validation/file-validation.directive';
import { CloudIconComponent } from '../../../icons/cloud-icon.component';
import { UploadIconComponent } from '../../../icons/upload-icon.component';
import { WORKSPACE_SERVICE } from '../../../services/workspace/workspace-service.token';

@Component({
  selector: 'app-create-document',
  imports: [ReactiveFormsModule, InputValidatorComponent, LoadingAnimationComponent, FailureCardComponent, CloudIconComponent, UploadIconComponent],
  templateUrl: './create-document.component.html'
})
export class CreateDocumentComponent {
  constructor(private modalService: NgxSmartModalService) { }

  isDragging = false;

  protected workspaceService = inject(WORKSPACE_SERVICE)

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave() {
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    if (event.dataTransfer?.files) {
      this.handleFiles(event.dataTransfer.files);
    }
  }

  onFileSelected(event: Event) {
    event.preventDefault();
    event.stopPropagation();

    const element = event.target as HTMLInputElement;

    if(element.files)
    {
      this.handleFiles(element.files);
    }
  }

  handleFiles(files : FileList) {
    if(files && files.length > 0)
    {
      this.file.setValue(files[0])
    }
  }

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
  documentValidationMessages = getRuleToMessageText('Document Name', 2, 50);
  descriptionValidationMessages = getRuleToMessageText('Description', 2, 50);
  fileValidationMessages = getRuleToMessageFile();
  workspaceValidationMessages = [{validationRule : 'required', errorMessage : 'Workspace is required'}]

  protected documentForm = new FormGroup(
    {
      documentName: new FormControl('', [
        Validators.required,
        Validators.maxLength(50),
        Validators.minLength(2)
      ]),
      description: new FormControl('', [
        Validators.required,
        Validators.maxLength(50),
        Validators.minLength(2)
      ]),
      file: new FormControl<File>(new File([], ''), [
        Validators.required,
        fileValidator()
      ]),
      workspaceId : new FormControl('',[
        Validators.required
      ])
    }
  )

  get documentName() {
    return this.documentForm.get('documentName') as FormControl<string | null>
  }

  get description() {
    return this.documentForm.get('description') as FormControl<string | null>
  }

  get file() {
    return this.documentForm.get('file') as FormControl<File>
  }

  get workspaceId () {
    return this.documentForm.get('workspaceId') as FormControl<string | null>
  }

  onSubmitForm() {
    if (this.documentForm.invalid) {
      return;
    }

    this.formState = 'loading';

    this.documentService.createDocument({
      workspaceId: this.workspaceId.value!,
      fileName: this.documentName.value!,
      description: this.description.value!,
      file: this.file.value!
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

