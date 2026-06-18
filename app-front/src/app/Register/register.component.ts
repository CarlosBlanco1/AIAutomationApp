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

  onSubmitForm() {
    this.authService.register(this.registerRequest);
  }

  registerForm = new FormGroup({
    firstName: new FormControl(this.registerRequest.firstName, [
      Validators.required,
      Validators.minLength(2),
    ]),
    lastName: new FormControl(this.registerRequest.lastName, [
      Validators.required,
      Validators.minLength(2),
    ]),
    email: new FormControl(this.registerRequest.email, [
      Validators.required,
      Validators.email,
    ]),
    password: new FormControl(this.registerRequest.password, [
      Validators.required,
      Validators.minLength(2),
    ]),
  });

  get firstName() {
    return this.registerForm.get('firstName') as FormControl<string|null>;
  }

  get lastName() {
    return this.registerForm.get('lastName') as FormControl<string|null>;
  }

  get email() {
    return this.registerForm.get('email') as FormControl<string|null>;
  }

  get password() {
    return this.registerForm.get('password') as FormControl<string|null>;
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
}
