import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/member-service';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { filter, Observable } from 'rxjs';
import { Member } from '../../../types/member';
import { AgePipe } from '../../../core/pipes/age-pipe';
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-member-detailed',
  imports: [AsyncPipe, RouterLink, RouterLinkActive, RouterOutlet, AgePipe],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css',
})
export class MemberDetailed implements OnInit {
  private accountService = inject(AccountService);
  protected memberService = inject(MemberService)
  private route = inject(ActivatedRoute) // gives us access to route parameters 
  private router = inject(Router)
  protected title = signal<string | undefined>('Profile')
  // protected member$?: Observable<Member>; now becomes the below seeing as we are not using loadMember anymore
  // protected member = signal<Member | undefined>(undefined);
  // derived signal 
  protected isCurrentUser = computed(() =>
    this.accountService.currentUser()?.id === this.route.snapshot.paramMap.get("id")
  )

  ngOnInit(): void {
    // this.member$ = this.loadMember();
    // this.route.data.subscribe({
    //   next: data => this.member.set(data['member'])
    // })
    this.title.set(this.route.firstChild?.snapshot?.title) // grab a hold of title provided in route (see app.route.ts where title is provided)

    // if the route changes we need to change the title too 
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd) // the event that contains the title change 
    ).subscribe({
      next: () => {
        this.title.set(this.route.firstChild?.snapshot?.title)
      }
    }
    );
  }

  // loadMember() {
  //   const id = this.route.snapshot.paramMap.get('id'); // see NOTES.md for more
  //   if (!id) return
  //   return this.memberService.getMember(id);
  // }
  // replaced with member-resolver.ts now
}
