import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ApiError } from '../../../types/error';

@Component({
  selector: 'app-server-error',
  imports: [],
  templateUrl: './server-error.html',
  styleUrl: './server-error.css',
})
export class ServerError {
  protected error: ApiError
  private router = inject(Router);
  protected showDetails = false;

  constructor() {
    const navigation = this.router.currentNavigation()
    // access the state provided when this component was navigated to 
    // only available when navigation takes place thus we can only ever access 
    // it in constructor when this component is created coming from a navigation 
    // for all other times when naivgation is idle - it will be null 
    this.error = navigation?.extras?.state?.['error']
  }

  detailsToggle() {
    this.showDetails = true;
  }
}
