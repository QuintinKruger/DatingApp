import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from '../likes-service';
import { PrecenseService } from '../precense-service';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root', // provided to our application as a whole
}) // can use DI from angular for this class as singleton 
export class AccountService {
  private http = inject(HttpClient); // dependency inject this dependency into this service
  private likesService = inject(LikesService);
  currentUser = signal<User | null>(null);
  private baseUrl = environment.apiUrl
  private precenseService = inject(PrecenseService);

  register(creds: RegisterCreds) {
    return this.http.post<User>(this.baseUrl + 'account/register', creds, { withCredentials: true }).pipe( // with credentials to get refresh token sent from server as cookie
      // allows side effect without updating the data
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
          this.startTokenRefreshInterval();
        }
      })
    )
  }

  login(creds: LoginCreds) {
    // adding in <User> to let compiler know what type we get back from post 
    // allows compilation of the code below where we make use of the `user`
    // data received 
    return this.http.post<User>(this.baseUrl + 'account/login', creds, { withCredentials: true }).pipe(
      // allows side effect without updating the data
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
          this.startTokenRefreshInterval();
        }
      })
    )
  }

  refreshToken() {
    return this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, {withCredentials: true}); // withCredentials sends cookie data to server
  }

  startTokenRefreshInterval() {
    setInterval(() => {
      this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, {withCredentials: true}).subscribe({ // withCredentials sends cookie data to server
        next: user => {
          this.setCurrentUser(user)
        },
        error: () => {
          this.logout();
        }
      })
    }, 5 * 60 * 1000) // every 5 mins   
  }

  setCurrentUser(user: User) {
    user.roles = this.getRolesFromToken(user)
    // to persist user across page reloads - bad practice
    // localStorage.setItem('user', JSON.stringify(user))
    this.currentUser.set(user)
    this.likesService.getLikeIds()
    if (this.precenseService.hubConnection?.state !== HubConnectionState.Connected) {
      this.precenseService.createHubConnection(user);
    }
  }

  logout() {
    localStorage.removeItem("filters")
    this.likesService.clearLikeIds()
    this.currentUser.set(null);
    this.precenseService.stopHubConnection();
  }

  private getRolesFromToken(user: User): string[] {
    const payload = user.token.split(".")[1];
    const decoded = atob(payload) // JS function to decode base 64 JSON token
    const jsonPayload = JSON.parse(decoded);
    return Array.isArray(jsonPayload.role) ? jsonPayload.role : [jsonPayload.role]; // 1 role is single value thus this trickery 
  }
}
