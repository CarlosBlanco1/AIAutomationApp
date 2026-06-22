import { Component, inject } from '@angular/core';
import { LogoComponent } from '../Logo/logo.component';
import { EmailIconComponent } from '../Icons/email-icon.component';
import { LockIconComponent } from '../Icons/lock-icon.component';
import { UserIconComponent } from '../Icons/user-icon.component';
import { AUTH_SERVICE } from '../Services/Auth/auth-service.token';
import { type CreateUserRequest } from '../Models/Users/create-user-request';
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
import { FailureIconComponent } from '../Icons/failure-icon.component';
import { getRuleToMessageEmail, getRuleToMessagePassword, getRuleToMessageText } from '../Dictionaries/validation-messages';

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
    FailureIconComponent
  ],
})
export class RegisterComponent {
  private authService = inject(AUTH_SERVICE);

  formState : RegistrationState = 'form'

  registerRequest: CreateUserRequest = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
  };

  errorMessage = 'failure'
  isLoading = false

  passwordValidationMessages = getRuleToMessagePassword();
  emailValidationMessages = getRuleToMessageEmail();
  fnValidationMessages = getRuleToMessageText('First Name')
  lnValidationMessages = getRuleToMessageText('Last Name')

  onSubmitForm() {
    if (this.registerForm.invalid) {
      return;
    }

    this.formState = 'loading'

    this.authService
      .register({
        firstName: this.firstName!.value!,
        lastName: this.lastName!.value!,
        email: this.email!.value!,
        password: this.password!.value!,
      })
      .subscribe({
        next: () => {
          this.formState = 'success'
        },
        error: (err) => {
          this.formState = 'failure'
          this.errorMessage = err
        },
      });
  }

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
}

type RegistrationState = 'form' | 'loading' | 'success' | 'failure';
