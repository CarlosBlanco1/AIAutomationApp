import { Component, inject } from '@angular/core';
import { LogoComponent } from '../Logo/logo.component';
import { EmailIconComponent } from '../Icons/email-icon.component';
import { LockIconComponent } from '../Icons/lock-icon.component';
import { UserIconComponent } from '../Icons/user-icon.component';
import { AUTH_SERVICE } from '../Services/Auth/auth-service.token';
import {
  FormControl,
  FormGroup,
  FormsModule,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { InputValidatorComponent } from './input-validator/input-validator.component';
import { securePasswordValidator } from '../Directives/Validation/password-validation.directive';
import { CheckIconComponent } from '../Icons/check-icon.component';
import { LoginIconComponent } from '../Icons/login-icon.component';
import { HouseIconComponent } from '../Icons/house-icon.component';
import { getRuleToMessageEmail, getRuleToMessageNewPassword, getRuleToMessageText } from '../Dictionaries/validation-messages';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { FailureCardComponent } from '../StateCards/failure-card/failure-card.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl : './register.component.css',
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
    FailureCardComponent
],
})
export class RegisterComponent {
  private authService = inject(AUTH_SERVICE);

  formState: RegistrationState = 'form'

  errorMessage = ''
  isLoading = false

  passwordValidationMessages = getRuleToMessageNewPassword();
  emailValidationMessages = getRuleToMessageEmail();
  fnValidationMessages = getRuleToMessageText('First Name')
  lnValidationMessages = getRuleToMessageText('Last Name')

  
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
