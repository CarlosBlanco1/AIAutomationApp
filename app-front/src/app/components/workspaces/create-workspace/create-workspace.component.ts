import { Component, EventEmitter, inject, Output } from '@angular/core';
import { WORKSPACE_SERVICE } from '../../../services/workspace/workspace-service.token';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { getRuleToMessageText } from '../../../dictionaries/validation-messages';
import { FailureCardComponent } from "../../state-cards/failure-card/failure-card.component";
import { LoadingAnimationComponent } from "../../../animations/loading-animation/loading-animation.component";
import { CloseIconComponent } from "../../../icons/close-icon.component";
import { SparkleIconComponent } from "../../../icons/sparkle-icon.component";
import { InputValidatorComponent } from "../../register/input-validator/input-validator.component";
import { NgxSmartModalService } from 'ngx-smart-modal';

@Component({
  selector: 'app-create-workspace',
  imports: [ReactiveFormsModule, FailureCardComponent, LoadingAnimationComponent, CloseIconComponent, SparkleIconComponent, InputValidatorComponent],
  templateUrl: './create-workspace.component.html'
})
export class CreateWorkspaceComponent {

  constructor(private modalService: NgxSmartModalService) { }

  onCancel() {
    this.modalService.get('createWorkspace').close();
  }

  onSuccess() {
    this.modalService.get('createWorkspace').close();
  }

  private workspaceService = inject(WORKSPACE_SERVICE)

  protected formState: createWorkspaceFormState = 'form';

  errorMessage = ''
  textValidationMessages = getRuleToMessageText('Workspace Name', 2, 50);

  protected workspaceForm = new FormGroup(
    {
      workspaceName: new FormControl('', [
        Validators.required,
        Validators.maxLength(50),
        Validators.minLength(2)
      ])
    }
  )

  get workspaceName() {
    return this.workspaceForm.get('workspaceName') as FormControl<string | null>
  }

  onSubmitForm() {
    if (this.workspaceForm.invalid) {
      return;
    }

    this.formState = 'loading';

    this.workspaceService.createWorkspace({
      workspaceName: this.workspaceName.value!
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
    this.workspaceForm.reset();
    this.workspaceForm.markAsPristine();
    this.workspaceForm.markAsUntouched();
    this.formState = 'form';
  }
}

type createWorkspaceFormState = 'form' | 'failure' | 'loading'
