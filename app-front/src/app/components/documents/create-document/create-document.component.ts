import { Component, inject } from '@angular/core';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { DOCUMENT_SERVICE } from '../../../services/document/document-service.token';
import { getRuleToMessageFile, getRuleToMessageText } from '../../../dictionaries/validation-messages';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SparkleIconComponent } from "../../../icons/sparkle-icon.component";
import { InputValidatorComponent } from "../../register/input-validator/input-validator.component";
import { LoadingAnimationComponent } from "../../../animations/loading-animation/loading-animation.component";
import { FailureCardComponent } from "../../state-cards/failure-card/failure-card.component";
import { fileValidator } from '../../../directives/Validation/file-validation.directive';

@Component({
  selector: 'app-create-document',
  imports: [ReactiveFormsModule, InputValidatorComponent, LoadingAnimationComponent, FailureCardComponent],
  templateUrl: './create-document.component.html'
})
export class CreateDocumentComponent {
  constructor(private modalService: NgxSmartModalService) { }

  isDragging = false;

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

    console.log(this.file.value)
    console.log(this.file.errors)
    console.log(this.file.invalid)
    console.log(this.file.dirty)
    console.log(this.file.touched)
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

  onSubmitForm() {
    if (this.documentForm.invalid) {
      return;
    }

    this.formState = 'loading';

    this.documentService.createDocument({
      workspaceId: '',
      fileName: '',
      description: '',
      file: new File([], '')
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

