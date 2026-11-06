import { Component, effect, ElementRef, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { MessageService } from '../../../core/message-service';
import { MemberService } from '../../../core/member-service';
import { Message } from '../../../types/message';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { FormsModule } from '@angular/forms';
import { PrecenseService } from '../../../core/precense-service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe, TimeAgoPipe, FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css',
})
export class MemberMessages implements OnInit, OnDestroy {
  @ViewChild('messageEndRef') messageEndRef!: ElementRef // when component is being created, the template is not yet ready thus this element could be undefined until such time
  protected messageService = inject(MessageService);
  private memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  protected messageContent = '';
  protected presenceService = inject(PrecenseService)

  constructor() {
    effect(() => { // side effect based on a signal that chages inside of the definition of the effect
      const currentMessages = this.messageService.messageThread();
      if (currentMessages.length > 0) {
        this.scrollToBottom()
      }
    }) 
  }

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe({
      next: params => {
        const otherUserId = params.get('id')
        if (!otherUserId) throw Error("Cannot connect to hub")
        this.messageService.createHubConnection(otherUserId)
        
      }
    })
  }

  // loadMessages() {
  //   const memberId = this.memberService.member()?.id // or could have gotten it from route params
  //   if (memberId) {
  //     this.messageService.getMessageThread(memberId).subscribe({
  //       next: (messages) => this.messages.set(messages.map(message => ({ 
  //         ...message, 
  //         currentUserSender: message.senderId !== memberId 
  //       })))
  //     })
  //   }
  // }

  sendMessage() {
    const recipientId = this.memberService.member()?.id;
    if (!recipientId) return;
    this.messageService.sendMessage(recipientId, this.messageContent)?.then(() => {
      this.messageContent = '';
    })
  }

  scrollToBottom() {
    setTimeout(() => { // because of note above regarding the template not yet being ready when component is created, only after a while 
      if(this.messageEndRef) {
      this.messageEndRef.nativeElement.scrollIntoView({behaviour: 'smooth'})
    }
    })
    
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

}
