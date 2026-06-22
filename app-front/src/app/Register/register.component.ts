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

  registerRequest: CreateUserRequest = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
  };

  registrationSucess : boolean | undefined = undefined
  errorMessage = 'failure'

  onSubmitForm() {
    if (this.registerForm.invalid) {
      return;
    }

    this.authService
      .register({
        firstName: this.registerForm.get('firstName')!.value!,
        lastName: this.registerForm.get('lastName')!.value!,
        email: this.registerForm.get('email')!.value!,
        password: this.registerForm.get('password')!.value!,
      })
      .subscribe({
        next: () => {
          this.registrationSucess = true;
        },
        error: () => {
          this.registrationSucess = false;
          this.errorMessage = "failed"
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

  getRuleToMessageText(inputName: string) {
    return [
      {
        validationRule: 'required',
        errorMessage: `${inputName} is required.`,
      },
      {
        validationRule: 'minlength',
        errorMessage: `${inputName} must be at least 2 characters long.`,
      },
      {
        validationRule: 'maxlength',
        errorMessage: `${inputName} must be no longer than 20 characters long.`,
      },
    ];
  }

  getRuleToMessageEmail() {
    return [
      {
        validationRule: 'required',
        errorMessage: `Email is required.`,
      },
      {
        validationRule: 'email',
        errorMessage: `Invalid email format.`,
      },
    ];
  }

  getRuleToMessagePassword() {
    return [
      {
        validationRule: 'required',
        errorMessage: `Password is required.`,
      },
      {
        validationRule: 'minlength',
        errorMessage: `Password must be at least 12 characters long.`,
      },
      {
        validationRule: 'maxlength',
        errorMessage: `Password must be no longer than 128 characters long.`,
      },
      {
        validationRule: 'notContainsSymbol',
        errorMessage: `Password must contain at least 1 symbol.`,
      },
      {
        validationRule: 'notContainsUpperAndLowerCase',
        errorMessage: `Password must contain at least 1 upper case and 1 lower case letter.`,
      },
      {
        validationRule: 'notContainsNumber',
        errorMessage: `Password must contain at least 1 number.`,
      },
    ];
  }
}
