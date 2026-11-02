import { Component, HostListener, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EditableMember, Member } from '../../../types/member';
import { DatePipe } from '@angular/common';
import { MemberService } from '../../../core/member-service';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastService } from '../../../core/services/toast-service';
import { AccountService } from '../../../core/services/account-service';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';

@Component({
  selector: 'app-member-profile',
  imports: [DatePipe, FormsModule, TimeAgoPipe],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css',
})
export class MemberProfile implements OnInit, OnDestroy {
  // allows access to something by its template reference variable 
  // in the template (member-profile.html)
  @ViewChild('editForm') editForm?: NgForm;
  // for this component route we have added a deactivate guard in the event the 
  // user navigates away from this component within our app - however this guard 
  // provides no protection when the user navigates outside of our application to 
  // somewhere else - the below magic basically adds the same deactivgate gaurd 
  // to the browser 
  @HostListener('window:beforeunload', ['$event'])
  notify($event: BeforeUnloadEvent) {
    if (this.editForm?.dirty) {
      $event.preventDefault();
    }
  }
  private route = inject(ActivatedRoute)
  private toast = inject(ToastService)
  private accountService = inject(AccountService);

  // protected member = signal<Member | undefined>(undefined);
  protected memberService = inject(MemberService)
  protected editableMember: EditableMember = {
    displayName: '',
    description: '',
    city: '',
    country: ''
  }

  ngOnInit(): void {
    // get member data from parent added to route as a resovler
    // this.route.parent?.data.subscribe(data => {
    //   this.member.set(data['member'])
    // })

    this.editableMember = {
      displayName: this.memberService.member()?.displayName || '',
      description: this.memberService.member()?.description || '',
      city: this.memberService.member()?.city || '',
      country: this.memberService.member()?.country || '',

    }
  }

  ngOnDestroy(): void {
    if (this.memberService.editMode()) {
      this.memberService.editMode.set(false)
    }
  }

  updateProfile() {
    if (!this.memberService.member()) return
    // take all member properties, override it with editableMember properties that may have been set
    const updatedMember = { ...this.memberService.member(), ...this.editableMember }
    this.memberService.updateMember(this.editableMember).subscribe({
      next: () => {
        const currentUser = this.accountService.currentUser();
        if (currentUser && updatedMember.displayName !== currentUser?.displayName) {
          currentUser.displayName = updatedMember.displayName
          this.accountService.setCurrentUser(currentUser)
        }
        this.toast.info("Profile updated successfully")
        this.memberService.editMode.set(false)
        this.memberService.member.set(updatedMember as Member);
        this.editForm?.reset(updatedMember); // editForm form's dirty flag gets reset to false 
      }
    })

  }

}
