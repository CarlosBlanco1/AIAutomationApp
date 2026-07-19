import { Component, inject } from "@angular/core";
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from "@angular/forms";
import { InputValidatorComponent } from "../register/input-validator/input-validator.component";
import { HttpErrorResponse } from "@angular/common/http";
import { FailureCardComponent } from "../state-cards/failure-card/failure-card.component";
import { Router, RouterLink } from "@angular/router";
import { ArrowIconComponent } from "../../icons/arrow-icon.component";
import { EmailIconComponent } from "../../icons/email-icon.component";
import { HouseIconComponent } from "../../icons/house-icon.component";
import { LockIconComponent } from "../../icons/lock-icon.component";
import { AUTH_SERVICE } from "../../services/auth/auth-service.token";
import { getRuleToMessageEmail, getRuleToMessageExistingPassword } from "../../dictionaries/validation-messages";
import { LoadingAnimationComponent } from "../../animations/loading-animation/loading-animation.component";
import { USER_SERVICE } from "../../services/user/user-service.token";

@Component({
    selector: 'app-login',
    standalone: true,
    templateUrl: './logincard.component.html',
    imports: [EmailIconComponent, LockIconComponent, ArrowIconComponent, InputValidatorComponent, ReactiveFormsModule, FailureCardComponent, HouseIconComponent, LoadingAnimationComponent, RouterLink]
})

export class LoginCardComponent {
    private authService = inject(AUTH_SERVICE)
    private userService = inject(USER_SERVICE);
    private router = inject(Router);

    protected formState: loginFormState = 'form';

    errorMessage = ''
    isLoading = false

    emailValidationMessages = getRuleToMessageEmail();
    passwordValidationMessages = getRuleToMessageExistingPassword();

    protected loginForm = new FormGroup({
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

        this.formState = 'loading';

        this.authService.login({
            email: this.email.value!,
            password: this.password.value!
        }).subscribe({
            next: () => {
                this.userService.fetchCurrentUser().subscribe({
                    complete: () => {
                        this.router.navigate(['/dashboard'])
                    }
                })
            },
            error: (err: HttpErrorResponse) => {
                this.formState = 'failure';
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
        this.formState = 'form';
    }
}

type loginFormState = 'form' | 'failure' | 'loading'