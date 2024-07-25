import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input()
  member: Member | undefined;

  @Input()
  liked = false;

  /**
   * Initializes a new instance of the MemberCardComponent.
   * @param memberService - The MembersService for managing member data.
   * @param toastr - The ToastrService for displaying notifications.
   * @param presenceService - The PresenceService for handling presence-related functionality.
   */
  constructor(
    private memberService: MembersService,
    private toastr: ToastrService,
    public presenceService: PresenceService
  ) {}

  /**
   * Angular lifecycle hook that initializes the component.
   */
  ngOnInit(): void {}

  /**
   * Adds a like to the specified member and updates the UI.
   * @param member - The member to like.
   */
  addLike(member: Member) {
    this.memberService.addLike(member.userName).subscribe({
      next: () => {
        this.toastr.success('You have liked ' + member.knownAs);
        this.liked = true;
      },
      error: (error) => {
        this.toastr.error('Failed to like ' + member.knownAs);
        console.error('Error liking member:', error);
      }
    });
  }
}
