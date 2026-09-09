import { Component, inject, OnDestroy } from '@angular/core';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { DOCUMENT_SERVICE } from '../../../services/document/document-service.token';
import { getRuleToMessageFile, getRuleToMessageText } from '../../../dictionaries/validation-messages';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputValidatorComponent } from "../../register/input-validator/input-validator.component";
import { fileValidator } from '../../../directives/Validation/file-validation.directive';
import { CloudIconComponent } from '../../../icons/cloud-icon.component';
import { UploadIconComponent } from '../../../icons/upload-icon.component';
import { WORKSPACE_SERVICE } from '../../../services/workspace/workspace-service.token';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-create-document',
  imports: [ReactiveFormsModule, InputValidatorComponent, CloudIconComponent, UploadIconComponent],
  templateUrl: './create-document.component.html'
})
export class CreateDocumentComponent implements OnDestroy {
  constructor(private modalService: NgxSmartModalService) { }

  private destroy$ = new Subject<void>();
  private cancelDocumentCreate$ = new Subject<void>();

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
    this.cancelDocumentCreate$.next();
    console.log("Operation cancelled")
    this.modalService.get('createDocument').close();
  }

  onSuccess() {
    this.documentService.getUserDocuments()
    .pipe(takeUntil(this.destroy$))
    .subscribe();
    this.modalService.get('createDocument').close();
  }

  private documentService = inject(DOCUMENT_SERVICE)

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

    this.documentService.createDocument({
      workspaceId: this.workspaceId.value!,
      fileName: this.documentName.value!,
      description: this.description.value!,
      file: this.file.value!
    })
    .pipe(
      takeUntil(this.cancelDocumentCreate$),
      takeUntil(this.destroy$)
    )
    .subscribe(
      {
        next: () => {
          this.onSuccess()
        },
        error: (err) => {
          if (err.error && typeof err.error === 'object') {
            this.errorMessage = err.error.message || 'An error occurred';
          } else {
            this.errorMessage = err.error
          }
        }
      }
    )
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.cancelDocumentCreate$.complete();
  }
}
