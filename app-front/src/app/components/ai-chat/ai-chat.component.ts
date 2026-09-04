import { Component, ElementRef, inject, Input, OnDestroy, signal, ViewChild } from "@angular/core";
import { SignalRService } from "../../services/signalr/signalr.service";
import { SparkleIconComponent } from "../../icons/sparkle-icon.component";
import { RefreshIconComponent } from "../../icons/refresh-icon.component";
import { PointerRightIconComponent } from "../../icons/pointer-right-icon.component";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { LoadingGoey } from "../../animations/loading-goey/loading-goey.component";
import { Subject, takeUntil } from "rxjs";

@Component({
    selector: 'app-ai-chat-component',
    templateUrl: './ai-chat.component.html',
    imports: [SparkleIconComponent, RefreshIconComponent, PointerRightIconComponent, ReactiveFormsModule, LoadingGoey]
})

export class AiChatComponent implements OnDestroy {
    @Input({ required: true }) documentId!: string;

    private signalrService = inject(SignalRService);
    messages = signal<(UserMessage | AIMessage | LoadingMessage)[]>([]);

    private destroy$ = new Subject<void>();

    private loadingInterval?: ReturnType<typeof setInterval>;

    isFirstCall = true;
    isInMiddleOfMessage = false;

    @ViewChild('messageContainer')
    private messageContainer!: ElementRef<HTMLDivElement>;

    constructor() {
        this.signalrService.startConnection();
        this.signalrService.addMessageListener()
        .pipe(takeUntil(this.destroy$))
        .subscribe({
            next: (chunk) => {

                if (this.loadingInterval) {
                    clearInterval(this.loadingInterval);
                    this.loadingInterval = undefined;
                }

                if (this.isFirstCall) {
                    this.messages.update((messages) => {
                        return [...messages.slice(0, -1), chunk]
                    })
                    this.isFirstCall = false;
                    requestAnimationFrame(() => {
                        this.scrollToBottom();
                    });
                    return;
                }

                if (chunk.done) {
                    this.isFirstCall = true;
                    this.isInMiddleOfMessage = false;
                }
                else {
                    this.messages.update((messages) => {

                        const updated = [...messages];
                        const lastIndex = updated.length - 1;

                        updated[lastIndex] = { ...updated[lastIndex], message: updated[lastIndex].message.concat(chunk.message) }

                        return updated;
                    })
                }

                requestAnimationFrame(() => {
                    this.scrollToBottom();
                });
            }
        })
    }

    protected messageForm = new FormGroup(
        {
            currentMessage: new FormControl('', [
                Validators.required,
                Validators.maxLength(100),
                Validators.minLength(4)
            ])
        }
    )

    ruleToMessage = [
        {
            validationRule: 'required',
            errorMessage: `Prompt is required.`,
        },
        {
            validationRule: 'minlength',
            errorMessage: `Prompt must be at least 4 characters long.`,
        },
        {
            validationRule: 'maxlength',
            errorMessage: `Prompt must be no longer than 100 characters long.`,
        },
    ]

    get currentMessage() {
        return this.messageForm.controls.currentMessage
    }

    sendMessage(event: SubmitEvent) {
        event.preventDefault();
        event.stopPropagation();
        
        if(this.isInMiddleOfMessage || this.messageForm.invalid) return;

        this.isInMiddleOfMessage = true;

        var newMesage: UserMessage = {
            id: crypto.randomUUID().toString(),
            sender: "User",
            message: this.currentMessage.value!,
            documentId: this.documentId
        }

        var newLoadingMessage: LoadingMessage = {
            id: crypto.randomUUID().toString(),
            sender: 'Loading',
            message: ''
        }

        this.signalrService.sendMessage(newMesage)

        this.messages.update(messages => [
            ...messages,
            newMesage,
            newLoadingMessage
        ])

        this.currentMessage.reset();

        this.loadingInterval = setInterval(() => {
            requestAnimationFrame(() => {
                this.scrollToBottom();
            })
        }, 200)
    }

    private scrollToBottom(): void {
        let container = this.messageContainer.nativeElement;

        container.scrollTo({
            top: container.scrollHeight,
            behavior: 'smooth'
        });
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();

        if(this.loadingInterval) {
            clearInterval(this.loadingInterval)
        }

        this.signalrService.stopConnection();
    }
}
export type UserMessage = {
    id: string,
    sender: 'User',
    message: string,
    documentId: string
}

export type AIMessage = {
    id: string,
    sender: 'AI',
    message: string,
    done: boolean
}

export type LoadingMessage = {
    id: string,
    sender: 'Loading',
    message: string
}
