import { Component, inject, OnInit, signal } from '@angular/core';
import { LikesService } from '../../core/likes-service';
import { Member } from '../../types/member';
import { MemberCard } from '../members/member-card/member-card';
import { Paginator } from '../../shared/paginator/paginator';
import { PaginatedResult } from '../../types/pagination';
import { LikesParams } from '../../types/likes';

@Component({
  selector: 'app-lists',
  imports: [MemberCard, Paginator],
  templateUrl: './lists.html',
  styleUrl: './lists.css',
})
export class Lists implements OnInit{
  private likesService = inject(LikesService);
  protected members = signal<Member[]>([])
  protected paginatedResult = signal<PaginatedResult<Member> | null>(null);
  protected likesParams : LikesParams = new LikesParams();

  tabs = [
    {label: 'Liked', value: 'liked'},
    {label: 'Liked me', value: 'likedBy'},
    {label: 'Mutual', value: 'mutual'}
  ]

  ngOnInit(): void {
    this.loadLikes();
  }

  setPredicate(predicate: string) {
    if (this.likesParams.predicate != predicate) {
      this.likesParams = new LikesParams();
      this.likesParams.predicate  = predicate
      this.loadLikes()
    }
  }

  loadLikes() {
     this.likesService.getLikes(this.likesParams).subscribe({
      next: result => this.paginatedResult.set(result)
     })
  }

  onPageChange($event: { pageNumber: number; pageSize: number; }) {
    this.likesParams.pageNumber = $event.pageNumber;
    this.likesParams.pageSize = $event.pageSize;
    this.loadLikes();
  }
  
}
