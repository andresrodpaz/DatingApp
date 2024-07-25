import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { SharedModule } from 'src/app/_modules/shared.module';
import { MembersService } from 'src/app/_services/members.service';
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
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?: User;

  /**
   * Initializes a new instance of the MemberDetailComponent.
   * @param memberService - The MembersService for fetching member data.
   * @param messageService - The MessageService for handling message-related operations.
   * @param route - The ActivatedRoute for accessing route parameters and data.
   * @param presenceService - The PresenceService for handling presence-related functionality.
   * @param accountService - The AccountService for managing user account data.
   */
  constructor(
    private memberService: MembersService,
    private messageService: MessageService,
    private route: ActivatedRoute,
    public presenceService: PresenceService,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user;
      }
    });
  }

  /**
   * Angular lifecycle hook that initializes the component.
   */
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    });

    this.route.queryParams.subscribe({
      next: params => {
        if (params['tab']) {
          this.selectedTab(params['tab']);
        }
      }
    });

    this.getImages();
  }

  /**
   * Handles tab activation and sets up or tears down the message hub connection based on the active tab.
   * @param data - The TabDirective containing information about the activated tab.
   */
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.user) {
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  /**
   * Loads the message thread for the current member.
   */
  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => {
          this.messages = messages;
          console.log('Loaded messages:', this.messages);
        },
        error: err => {
          console.error('Failed to load messages:', err);
        }
      });
    }
  }

  /**
   * Fetches the member data based on the username parameter from the route.
   */
  loadMember() {
    const username = this.route.snapshot.paramMap.get('username');
    if (username) {
      this.memberService.getMember(username).subscribe({
        next: member => {
          this.member = member;
          this.getImages();
        },
      });
    }
  }

  /**
   * Selects the specified tab in the tabset component.
   * @param heading - The heading of the tab to activate.
   */
  selectedTab(heading: string) {
    if (this.memberTabs && this.memberTabs.tabs.length > 0) {
      const tab = this.memberTabs.tabs.find(x => x.heading === heading);
      if (tab) {
        tab.active = true;
      }
    }
  }

  /**
   * Initializes the gallery images from the member's photos.
   */
  getImages() {
    if (this.member && this.member.photos) {
      this.images = this.member.photos.map(photo => new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }

  /**
   * Angular lifecycle hook that cleans up resources when the component is destroyed.
   */
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
