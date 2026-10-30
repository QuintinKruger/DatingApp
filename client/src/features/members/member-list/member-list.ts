import { Component, inject } from '@angular/core';
import { MemberService } from '../../../core/member-service';
import { Observable } from 'rxjs';
import { Member } from '../../../types/member';
import { AsyncPipe } from '@angular/common';
import { MemberCard } from '../member-card/member-card';

@Component({
  selector: 'app-member-list',
  imports: [AsyncPipe, MemberCard],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css',
})
export class MemberList {
  private memberService = inject(MemberService);
  protected members$:  Observable<Member[]>;// defining an observable has the naming convention of suffixing with $

  constructor () {
    this.members$ = this.memberService.getMembers();
  }
}
