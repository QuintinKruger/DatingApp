import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from '../likes-service';

@Injectable({
  providedIn: 'root', // provided to our application as a whole
}) // can use DI from angular for this class as singleton 
export class AccountService {
  private http = inject(HttpClient); // dependency inject this dependency into this service
  private likesService = inject(LikesService);
  currentUser = signal<User | null>(null);
  private baseUrl = environment.apiUrl

  register(creds: RegisterCreds) {
    return this.http.post<User>(this.baseUrl + 'account/register', creds).pipe(
      // allows side effect without updating the data
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
        }
      })
    )
  }

  login(creds: LoginCreds) {
    // adding in <User> to let compiler know what type we get back from post 
    // allows compilation of the code below where we make use of the `user`
    // data received 
    return this.http.post<User>(this.baseUrl + 'account/login', creds).pipe(
      // allows side effect without updating the data
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
        }
      })
    )
  }

  setCurrentUser(user: User) {
    // to persist user across page reloads
    localStorage.setItem('user', JSON.stringify(user))
    this.currentUser.set(user)
    this.likesService.getLikeIds()
  }

  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem("filters")
    this.likesService.clearLikeIds()
    this.currentUser.set(null);
  }
}
