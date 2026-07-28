import { inject, Injectable, WritableSignal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AppConfigService } from '../configuration/app-config.service';
import { AIMessage, UserMessage } from '../../components/ai-chat/ai-chat.component';
import { Observable, Subscriber } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection?: signalR.HubConnection;
  private configService = inject(AppConfigService);

  constructor() { }

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.configService.apiUrl}/chathub`, {
        withCredentials: false
      })
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connection started'))
      .catch(err => console.log('Error establishing SignalR connection: ' + err));
  }

  public addMessageListener(): Observable<AIMessage> {
    return new Observable<AIMessage>(subscriber => {
      if (!this.hubConnection) {
        subscriber.error(new Error("Connection hasn't been initialized"))
      }

      const handler = (message: AIMessage) => {
        subscriber.next(message)
      }

      this.hubConnection?.on("ReceiveMessage", handler)

      return () => {
        this.hubConnection?.off("ReceiveMessage", handler)
      }
    })
  };

  public sendMessage = (message: UserMessage) => {
    this.hubConnection?.invoke('SendMessage', message)
      .catch(err => console.error(err));
  }
}

