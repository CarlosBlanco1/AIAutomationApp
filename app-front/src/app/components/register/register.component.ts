import { Component, inject } from '@angular/core';
import { LogoComponent } from '../../logo/logo.component';
import { EmailIconComponent } from '../../icons/email-icon.component';
import { LockIconComponent } from '../../icons/lock-icon.component';
import { UserIconComponent } from '../../icons/user-icon.component';
import { AUTH_SERVICE } from '../../services/auth/auth-service.token';
import {
  FormControl,
  FormGroup,
  FormsModule,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { InputValidatorComponent } from './input-validator/input-validator.component';
import { securePasswordValidator } from '../../directives/Validation/password-validation.directive';
import { CheckIconComponent } from '../../icons/check-icon.component';
import { LoginIconComponent } from '../../icons/login-icon.component';
import { HouseIconComponent } from '../../icons/house-icon.component';
import { getRuleToMessageEmail, getRuleToMessageNewPassword, getRuleToMessageText } from '../../dictionaries/validation-messages';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { FailureCardComponent } from '../state-cards/failure-card/failure-card.component';
import { LoadingAnimationComponent } from "../../animations/loading-animation/loading-animation.component";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  standalone: true,
  imports: [
    LogoComponent,
    EmailIconComponent,
    LockIconComponent,
    UserIconComponent,
    FormsModule,
    ReactiveFormsModule,
    InputValidatorComponent,
    CheckIconComponent,
    LoginIconComponent,
    HouseIconComponent,
    LockIconComponent,
    RouterLink,
    FailureCardComponent,
    LoadingAnimationComponent
],
})
export class RegisterComponent {
  private readonly authService = inject(AUTH_SERVICE);

  formState: RegistrationState = 'form'

  errorMessage = ''
  isLoading = false

  passwordValidationMessages = getRuleToMessageNewPassword();
  emailValidationMessages = getRuleToMessageEmail();
  fnValidationMessages = getRuleToMessageText('First Name', 2, 20)
  lnValidationMessages = getRuleToMessageText('Last Name', 2, 20)

  
  registerForm = new FormGroup({
    firstName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(20),
    ]),
    lastName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(20),
    ]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(12),
      Validators.maxLength(128),
      securePasswordValidator(),
    ]),
  });
  
  get firstName() {
    return this.registerForm.get('firstName') as FormControl<string | null>;
  }
  
  get lastName() {
    return this.registerForm.get('lastName') as FormControl<string | null>;
  }
  
  get email() {
    return this.registerForm.get('email') as FormControl<string | null>;
  }
  
  get password() {
    return this.registerForm.get('password') as FormControl<string | null>;
  }

  onSubmitForm() {
    if (this.registerForm.invalid) {
      return;
    }
    
    this.formState = 'loading'
    
    this.authService
    .register({
      firstName: this.firstName.value!,
      lastName: this.lastName.value!,
      email: this.email.value!,
      password: this.password.value!,
    })
    .subscribe({
      next: () => {
        this.formState = 'success'
      },
      error: (err: HttpErrorResponse) => {
        this.formState = 'failure'
        if (err.error && typeof err.error === 'object') {
          this.errorMessage = err.error.message || 'An error occurred';
        } else {
          this.errorMessage = err.error
        }
      }
    });
  }

  tryAgain() {
    this.registerForm.reset();
    this.registerForm.markAsPristine();
    this.registerForm.markAsUntouched();
    this.formState = 'form';
  }
}

type RegistrationState = 'form' | 'loading' | 'success' | 'failure';
