import { inject, Injectable, signal } from '@angular/core';
import { ToastService } from './services/toast-service';
import { User } from '../types/user';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr'
import { environment } from '../environments/environment';
import { Message } from '../types/message';
@Injectable({
  providedIn: 'root',
})
export class PrecenseService {
  private hubUrl = environment.hubUrl
  private toast = inject(ToastService);
  public hubConnection?: HubConnection
  public onlineUsers = signal<string[]>([])

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token // passes token to server as query string parameter when negotiating with server to connect
      })
      .withAutomaticReconnect()
      .build();
    this.hubConnection.start()
      .catch(error => console.log(error))

    // callback function configured on server, defining the callback to be called here 
    this.hubConnection.on("UserOnline", userId => {
      console.log("UserOnline")
      this.onlineUsers.update(users => [...users, userId])
    })
    this.hubConnection.on("UserOffline", userId => {
      console.log("UserOffline")
      this.onlineUsers.update(users => users.filter(x => x !== userId))
    })
    this.hubConnection.on("GetOnlineUsers", userIds => {
      console.log("GetOnlineUsers")
      this.onlineUsers.set(userIds);
    })
    this.hubConnection.on("NewMessageReceived", (message: Message) => {
      this.toast.info(message.senderDisplayName + " has sent you a new message",
        10000, message.senderImageUrl,
        `/members/${message.senderId}/messages`)
    })
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop()
        .catch(error => console.log(error))
    }
  }

}
