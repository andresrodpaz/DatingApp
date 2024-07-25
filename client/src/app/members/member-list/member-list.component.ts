import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members: Member[] = [];
  pagination: Pagination = { currentPage: 1, itemsPerPage: 10, totalItems: 0, totalPages: 0 };
  userParams: UserParams | undefined;
  user: User | undefined;
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
    { value: 'non-binary', display: 'Non-Binary' },
    { value: 'other', display: 'Others' }
  ];

  /**
   * Initializes the MemberListComponent with the MembersService.
   * @param memberSvc - The MembersService for managing member data.
   */
  constructor(private memberSvc: MembersService) {
      this.userParams = this.memberSvc.getUserParams();
  }

  /**
   * Angular lifecycle hook that initializes the component.
   * Loads the list of members based on current user parameters.
   */
  ngOnInit(): void {
    this.loadMembers();
  }

  /**
   * Loads members based on user parameters.
   * Updates the members list and pagination information upon receiving response.
   */
  loadMembers() {
    if (this.userParams) {
      this.memberSvc.setUserParams(this.userParams);
      this.memberSvc.getMembers(this.userParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      });
    }
  }

  /**
   * Resets user filters to default values and reloads members.
   */
  resetFilters() {
    if (this.user) {
      this.userParams = this.memberSvc.resetUserParams();
      this.loadMembers();
    }
  }

  /**
   * Handles page changes in pagination.
   * Updates the user parameters and reloads members for the selected page.
   * @param event - The pagination change event.
   */
  pageChanged(event: any) {
    if (this.userParams && this.userParams.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.memberSvc.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
