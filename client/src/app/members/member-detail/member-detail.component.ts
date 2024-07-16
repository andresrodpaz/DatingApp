import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { SharedModule } from 'src/app/_modules/shared.module';
import { MembersService } from 'src/app/_services/members.service';
import { CustomDateParser } from 'src/app/pipes/custom-date-parser.pipe';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-member-details',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [
    CommonModule,
    TabsModule,
    GalleryModule,
    TimeagoModule,
    SharedModule,
    MemberMessagesComponent,
  ],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static:true}) memberTabs?: TabsetComponent;
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?:User;

  constructor(
    private memberService: MembersService, private messageService: MessageService,  private route: ActivatedRoute,
    public presenceService : PresenceService, private accountService : AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user) this.user = user;
      }
    })
  }

  ngOnInit(): void {

    this,this.route.data.subscribe({
      next: data => this.member = data['member']
    })

    this.route.queryParams.subscribe({
      next: params =>{
        params['tab'] && this.selectedTab(params['tab'])
      }
    })
    this.getImages();
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.user) {
      this.messageService.createHubConnection(this.user, this.member.userName)
    } else {
      this.messageService.stopHubConnection();
    }
  }

  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: (messages) => {
          this.messages = messages;
          console.log('Loaded messages:', this.messages);
        },
        error: (err) => {
          console.error('Failed to load messages:', err);
        }
      });
    }
  }


  loadMember() {
    const username = this.route.snapshot.paramMap.get('username');

    if (!username) return;

    this.memberService.getMember(username).subscribe({
      next: (member) => {
        (this.member = member), this.getImages();
      },
    });
  }

  selectedTab(heading: string) {
    if (this.memberTabs && this.memberTabs.tabs && this.memberTabs.tabs.length > 0) {
      const tab = this.memberTabs.tabs.find(x => x.heading === heading);
      if (tab) {
        tab.active = true;
      }
    }
  }


  getImages() {
    if (!this.member || !this.member.photos) return;

    for (const photo of this.member.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
