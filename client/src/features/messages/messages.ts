import { Component, inject, OnInit, signal } from '@angular/core';
import { MessageService } from '../../core/message-service';
import { PaginatedResult } from '../../types/pagination';
import { Message } from '../../types/message';
import { Paginator } from '../../shared/paginator/paginator';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-messages',
  imports: [Paginator, RouterLink, DatePipe],
  templateUrl: './messages.html',
  styleUrl: './messages.css',
})
export class Messages implements OnInit {
  private messageService = inject(MessageService)
  protected container = "Inbox"
  protected fetchedContainer = "Inbox"
  protected pageSize = 10;
  protected pageNumber = 1
  protected paginatedMessages = signal<PaginatedResult<Message> | null>(null);

  tabs = [
    { label: 'Inbox', value: 'Inbox' },
    { label: 'Outbox', value: 'Outbox' },
  ]

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.messageService.getMessages(this.container, this.pageNumber, this.pageSize).subscribe({
      next: paginatedResponse =>{ 
        this.paginatedMessages.set(paginatedResponse)
        this.fetchedContainer = this.container // only set the container to force toggle to new tab once data for it has loaded - prevents flicker from old data to new
      }
    })
  }

  deleteMessage(event: Event, id: string) {
    event.stopPropagation();
    this.messageService.deleteMessage(id).subscribe({
      next: () => {
        const current = this.paginatedMessages(); // copy of current messages
        if  (current?.items) {
          this.paginatedMessages.update(prev => {
            if (!prev) return null;

            const newItems = prev.items.filter(x => x.id !== id) || []

            return {
              items: newItems,
              metadata: prev.metadata // old meta data, skipped this for complexity - we could just reload messages in this next method or we can handle the change of meta data ourselves in this method - both options deferred
            }
          })
        }
      }
    })
  }

  get isInbox() {
    return this.fetchedContainer === 'Inbox'
  }

  setContainer(container: string) {
    this.container = container;
    this.pageNumber = 1; // reset page when the other container is set
    this.loadMessages();
  }

  onPageChange($event: { pageNumber: number; pageSize: number; }) {
    this.pageNumber = $event.pageNumber;
    this.pageSize = $event.pageSize;
    this.loadMessages();
  }
}
