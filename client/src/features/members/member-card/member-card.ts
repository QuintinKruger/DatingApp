import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../../types/member';
import { RouterLink } from '@angular/router';
import { AgePipe } from '../../../core/pipes/age-pipe';
import { LikesService } from '../../../core/likes-service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink, AgePipe],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css',
})
export class MemberCard {
  private likesService = inject(LikesService)
  member = input.required<Member>();
  protected hasLiked = computed(() => this.likesService.likeIds().includes(this.member().id))

  toggleLike(event: Event) {
    event.stopPropagation() // prevent click event from propogating to element that will navigate user to the profile of member (<div routerLink="/members/{{member.id}}".... in template)
    this.likesService.toggleLike(this.member().id).subscribe({
      next: () => {
        if (this.hasLiked()) {
          this.likesService.likeIds.update(ids => ids.filter(x => x !== this.member().id))
        } else {
          this.likesService.likeIds.update(ids => [...ids, this.member().id])
        }
      }
    })
  }
}
