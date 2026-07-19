import { Component, inject, signal } from "@angular/core";
import { SparkleIconComponent } from "../../icons/sparkle-icon.component";
import { UsersIconComponent } from "../../icons/users-icon.component";
import { GraphIconComponent } from "../../icons/graph-icon.component";
import { PointerRightIconComponent } from "../../icons/pointer-right-icon.component";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { CONFIRMATION_TOKEN_SERVICE } from "../../services/confirmation-token/confirmation-token-service.token";
import { USER_SERVICE } from "../../services/user/user-service.token";
import { LoadingAnimationComponent } from "../../animations/loading-animation/loading-animation.component";
import { FailureCardComponent } from "../state-cards/failure-card/failure-card.component";

@Component({
    selector: 'app-verification-success',
    templateUrl: './verification-succcess.component.html',
    imports: [SparkleIconComponent, UsersIconComponent, GraphIconComponent, PointerRightIconComponent, LoadingAnimationComponent, FailureCardComponent, RouterLink]
})
export class VerificationSuccessComponent {
    private route = inject(ActivatedRoute);
    private confirmationService = inject(CONFIRMATION_TOKEN_SERVICE);
    private userService = inject(USER_SERVICE);

    pageState = signal<EmailVerificationPageState>('loading');
    errorMessage = '';

    constructor() {
        const snapshot = this.route.snapshot;
        var queryParams = snapshot.queryParams;

        if (!Object.hasOwn(queryParams, 'userId') || !Object.hasOwn(queryParams, 'token')) {
            this.pageState.set('error');
            this.errorMessage = "Missing query parameters!";
        }
        else {
            var userId = queryParams['userId'];
            var token = queryParams['token'];

            this.confirmationService.validateConfirmationToken(userId, token)
                .subscribe({
                    next: () => {
                        this.userService.fetchCurrentUser().subscribe({
                            complete : () => {
                                this.pageState.set('success');
                            }
                        })
                    },
                    error: (err) => {
                        console.log(err)
                        this.pageState.set('error');
                        if (err.error && typeof err.error === 'object') {
                            this.errorMessage = err.error.message || 'An error occurred';
                        } else {
                            this.errorMessage = err.error
                        }
                    }
                })
        }
    }
}

type EmailVerificationPageState = 'loading' | 'success' | 'error';