import { Component, inject, Input, signal } from '@angular/core';
import { Register } from '../account/register/register';
import { User } from '../../types/user';
import { AccountService } from '../../core/services/account-service';

@Component({
  selector: 'app-home',
  imports: [Register],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  // @Input({required: true}) membersFromApp: User[] = [] ; // pass input to this child of App component (this is the childs input a parent must provide using this component)
  protected registerMode = signal(false);
  protected accountService = inject(AccountService);

  showRegsiter(value: boolean) {
    this.registerMode.set(value);
  }
}
