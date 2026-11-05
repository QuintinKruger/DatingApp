import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastService } from '../../core/services/toast-service';
import { themes } from '../theme';
import { BusyService } from '../../core/busy-service';
import { HasRole } from '../../shared/directives/has-role';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive, HasRole], // to use templated angular forms in html 
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav implements OnInit {
  protected busyService = inject(BusyService);
  protected accountService: any = inject(AccountService);
  private toastService = inject(ToastService)
  private router = inject(Router)
  protected creds: any = {}
  protected selectedTheme = signal<string>(localStorage.getItem('theme') || 'light');
  protected themes = themes

  ngOnInit(): void {
     document.documentElement.setAttribute('data-theme', this.selectedTheme()) // this is how we apply a theme in daisy UI
  }

  handleSelectTheme(theme: string) {
    this.selectedTheme.set(theme);
    localStorage.setItem('theme', theme);
    document.documentElement.setAttribute('data-theme', theme) // this is how we apply a theme in daisy UI
    
    // the active element at this point should be our dropdown
    // this will blur the drop down away when we select a theme 
    // this is daisy-ui's suggested method of removing the dropdown
    const elm = document.activeElement as HTMLDivElement
    if (elm) {
      elm.blur()
    }
  }

  login() {
    this.accountService.login(this.creds).subscribe({
      next: () => {
        this.router.navigateByUrl("/members");
        this.toastService.success("Logged in successfully")
        this.creds = {}
      },
      error: (error: any) => {
        this.toastService.error(error.error);
      }
    }
    )
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl("/");
  }
}
