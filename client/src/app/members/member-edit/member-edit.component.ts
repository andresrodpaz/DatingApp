import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm', { static: false }) editForm: NgForm | undefined;

  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }

  member: Member | undefined;
  user: User | null = null;

  /**
   * Initializes the component with services for managing account and member data.
   * @param accountSvc - The AccountService for managing user account data.
   * @param memberSvc - The MembersService for managing member data.
   * @param toastr - The ToastrService for displaying notifications.
   */
  constructor(
    private accountSvc: AccountService,
    private memberSvc: MembersService,
    private toastr: ToastrService
  ) {}

  /**
   * Angular lifecycle hook that initializes the component.
   * Loads the current member data when the component initializes.
   */
  ngOnInit(): void {
    this.loadMember();
  }

  /**
   * Loads the current user's member data.
   * Subscribes to the current user observable to get the user details
   * and then fetches the corresponding member data.
   */
  loadMember(): void {
    this.accountSvc.currentUser$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          this.user = user;
          this.memberSvc.getMember(user.username).subscribe({
            next: (member: Member | undefined) => {
              this.member = member;
            },
            error: (error: any) => {
              console.error('Error loading member: ', error);
            }
          });
        }
      },
      error: (error: any) => {
        console.error('Error loading current user: ', error);
      }
    });
  }

  /**
   * Updates the current member's profile information.
   * Subscribes to the updateMember method of the MembersService
   * and displays a success or error message based on the response.
   */
  updateMember(): void {
    if (!this.editForm || !this.editForm.valid || !this.member) {
      return;
    }
    console.log(this.member);
    this.memberSvc.updateMember(this.member).subscribe({
      next: () => {
        this.toastr.success('Profile updated successfully');
        if (this.editForm) {
          this.editForm.reset(this.member);
        }
      },
      error: (error: any) => {
        console.error('Error updating member: ', error);
        this.toastr.error('Failed to update profile');
      }
    });
  }
}
