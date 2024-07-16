import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { SharedModule } from 'src/app/_modules/shared.module';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent implements OnInit{

  @ViewChild('messageForm') messageForm? : NgForm;
  @Input() username?:string;

  messageContent = '';

  constructor(public messageService : MessageService, public presenceService : PresenceService){

  }
  ngOnInit(): void {

  }

  sendMessage() {
    if (!this.username) {
      console.error('Username is not defined');
      return;
    }

    if (!this.messageContent) {
      console.error('Message content is empty');
      return;
    }

    console.log('Sending message to username:', this.username);
    console.log('Message content:', this.messageContent);

    // this.messageService.sendMessage(this.username, this.messageContent).subscribe({
    //   next: message => {
    //     console.log('Message sent successfully:', message);
    //     console.log('Sender Photo URL:', message.senderPhotoUrl);
    //     console.log('Recipient Photo URL:', message.recipientPhotoUrl);

    //     // this.messages.push(message);
    //     // this.messageForm?.reset();
    //   },
    //   error: err => {
    //     console.error('Error sending message:', err);
    //   }
    // });
    this.messageService.sendMessage(this.username, this.messageContent).then(()=> {
      this.messageForm?.reset();
    })
  }




}
