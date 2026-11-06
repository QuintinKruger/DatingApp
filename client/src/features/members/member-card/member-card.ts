import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../../types/member';
import { RouterLink } from '@angular/router';
import { AgePipe } from '../../../core/pipes/age-pipe';
import { LikesService } from '../../../core/likes-service';
import { PrecenseService } from '../../../core/precense-service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink, AgePipe],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css',
})
export class MemberCard {
  private likesService = inject(LikesService)
  private presenceService = inject(PrecenseService);
  member = input.required<Member>();
  protected hasLiked = computed(() => this.likesService.likeIds().includes(this.member().id))
  protected isOnline = computed(() => this.presenceService.onlineUsers().includes(this.member().id))

  toggleLike(event: Event) {
    event.stopPropagation() // prevent click event from propogating to element that will navigate user to the profile of member (<div routerLink="/members/{{member.id}}".... in template)
    this.likesService.toggleLike(this.member().id)
  }
}
