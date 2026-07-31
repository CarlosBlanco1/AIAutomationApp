import { Component, ElementRef, inject, Input, signal, ViewChild } from "@angular/core";
import { SignalRService } from "../../services/signalr/signalr.service";
import { SparkleIconComponent } from "../../icons/sparkle-icon.component";
import { RefreshIconComponent } from "../../icons/refresh-icon.component";
import { PointerRightIconComponent } from "../../icons/pointer-right-icon.component";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";

@Component({
    selector: 'app-ai-chat-component',
    templateUrl: './ai-chat.component.html',
    imports: [SparkleIconComponent, RefreshIconComponent, PointerRightIconComponent, ReactiveFormsModule]
})

export class AiChatComponent {
    @Input({required : true}) documentId! : string;

    private signalrService = inject(SignalRService);
    messages = signal<(UserMessage | AIMessage)[]>([]);
    isFirstCall = true;

    @ViewChild('messageContainer')
    private messageContainer!: ElementRef<HTMLDivElement>;

    constructor() {
        this.signalrService.startConnection();
        this.signalrService.addMessageListener().subscribe({
            next: (chunk) => {

                if (this.isFirstCall) {
                    this.messages.update((messages) => {
                        return [...messages, chunk]
                    })
                    this.isFirstCall = false;
                    requestAnimationFrame(() => {
                        this.scrollToBottom();
                    });
                    return;
                }

                if (chunk.done) {
                    this.isFirstCall = true;
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
                Validators.maxLength(50),
                Validators.minLength(4)
            ])
        }
    )

    get currentMessage() {
        return this.messageForm.controls.currentMessage
    }

    sendMessage(event: SubmitEvent) {
        event.preventDefault();
        event.stopPropagation();
        var newMesage: UserMessage = {
            id: crypto.randomUUID().toString(),
            sender: "User",
            message: this.currentMessage.value!,
            documentId : this.documentId
        }

        this.signalrService.sendMessage(newMesage)

        this.messages.update(messages => [
            ...messages,
            newMesage
        ])

        requestAnimationFrame(() => {
            this.scrollToBottom();
        });

        this.currentMessage.reset();
    }

    private scrollToBottom(): void {
        const container = this.messageContainer.nativeElement;

        container.scrollTo({
            top: container.scrollHeight,
            behavior: 'smooth'
        });
    }
}

export type UserMessage = {
    id: string,
    sender: 'User',
    message: string,
    documentId : string
}

export type AIMessage = {
    id: string,
    sender: 'AI',
    message: string,
    done: boolean
}