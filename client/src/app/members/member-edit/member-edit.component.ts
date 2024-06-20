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

  constructor(
    private accountSvc: AccountService,
    private memberSvc: MembersService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadMember();
  }

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
