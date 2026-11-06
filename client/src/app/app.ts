import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Nav } from '../layout/nav/nav';
import { User } from '../types/user';
import { Router, RouterOutlet } from '@angular/router';
import { ConfirmDialog } from '../shared/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet, ConfirmDialog], // any component used in app.html is added here
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  // inject into this component - old style, below is new style
  // constructor(private http: HttpClient) {}
  private http = inject(HttpClient);
  protected router = inject(Router)
  protected title = 'Dating app'
  protected members = signal<User[]>([]); // signals are used for reactivity for html to receive updates. We initialize it with an empty array

  // hook into lifecycle of component
  async ngOnInit() {
    // natutally this will return an observable - a datatype that can receive updates 
    // to data that changes over time. Not necessarily used for http service where a request is made and a response is received 
    // this.http.get("https://localhost:5001/api/members").subscribe(
    //   {
    //     next: response => this.members.set(response),
    //     error: error => console.log(error),
    //     complete: () => console.log("Completed HTTP request") // because the http service only ever sends one piece of data we request, this will always run
    //                                                           // It will automatically unsubscribe from the observable and run this code - thus no need to unsubscribe from observable
    //   }
    // )
    // this.members.set(await this.getMembers())
  }

  // small chance that an http service does not run next or error and thus does not complete 
  // i.e. it then does not unsubscribe. We can cater for this small chance by converting 
  // our observable to a promise (a promise does not receive updates and can not be subscribed to)
  // which honestly makes more sense
  // async getMembers() {
  //   try {
  //     return firstValueFrom(this.http.get<User[]>("https://localhost:5001/api/members"))
  //   } catch (error)
  //   {
  //     console.log(error)
  //     throw error;
  //   }
      
  // }
}
