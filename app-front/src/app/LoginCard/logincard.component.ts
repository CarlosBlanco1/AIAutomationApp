import { Component, inject } from "@angular/core";
import { EmailIconComponent } from "../Icons/email-icon.component";
import { LockIconComponent } from "../Icons/lock-icon.component";
import { ArrowIconComponent } from "../Icons/arrow-icon.component";
import { AUTH_SERVICE } from "../Services/Auth/auth-service.token";
import { getRuleToMessageEmail, getRuleToMessageExistingPassword } from "../Dictionaries/validation-messages";
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from "@angular/forms";
import { InputValidatorComponent } from "../Register/input-validator/input-validator.component";
import { HttpErrorResponse } from "@angular/common/http";
import { FailureCardComponent } from "../StateCards/failure-card/failure-card.component";
import { HouseIconComponent } from "../Icons/house-icon.component";
import { Router } from "@angular/router";

@Component({
    selector: 'app-login',
    standalone: true,
    templateUrl: './logincard.component.html',
    imports: [EmailIconComponent, LockIconComponent, ArrowIconComponent, InputValidatorComponent, ReactiveFormsModule, FailureCardComponent, HouseIconComponent]
})

export class LoginCardComponent {
    private authService = inject(AUTH_SERVICE)
    private router = inject(Router);

    isInFormState = true

    errorMessage = ''
    isLoading = false

    emailValidationMessages = getRuleToMessageEmail();
    passwordValidationMessages = getRuleToMessageExistingPassword();

    loginForm = new FormGroup({
        email: new FormControl('', [
            Validators.required,
            Validators.email
        ]),
        password: new FormControl('', [
            Validators.required
        ])
    })

    get email() {
        return this.loginForm.get('email') as FormControl<string | null>
    }

    get password() {
        return this.loginForm.get('password') as FormControl<string | null>
    }

    onSubmitForm() {

        if (this.loginForm.invalid) {
            return;
        }

        this.isInFormState = false;

        this.authService.login({
            email: this.email.value!,
            password: this.password.value!
        }).subscribe({
            next: (res) => {
                //Store token in local storage
                console.log(res.jwtToken)
                this.router.navigateByUrl('/')
            },
            error: (err: HttpErrorResponse) => {
                this.isInFormState = false;
                if (err.error && typeof err.error === 'object') {
                    this.errorMessage = err.error.message || 'An error occurred';
                } else {
                    this.errorMessage = err.error
                }
            }
        })
    }

    tryAgain() {
        this.loginForm.reset();
        this.loginForm.markAsPristine();
        this.loginForm.markAsUntouched();
        this.isInFormState = true;
    }

}