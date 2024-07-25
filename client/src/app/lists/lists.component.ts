import { Component, Input, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  members: Member[] | undefined;
  predicate: string = 'liked';

  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination | undefined;

  @Input()
  liked = false;

  /**
   * Initializes a new instance of the ListsComponent.
   * @param memberSvc - The MembersService for fetching member data.
   */
  constructor(private memberSvc: MembersService) {}

  /**
   * Angular lifecycle hook that initializes the component.
   */
  ngOnInit(): void {
    this.loadLikes();
  }

  /**
   * Loads the list of liked members from the server.
   */
  loadLikes() {
    this.memberSvc.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe({
      next: response => {
        this.members = response.result;
        this.pagination = response.pagination;
      },
      error: error => {
        console.error('Error loading likes:', error);
      }
    });
  }

  /**
   * Handles page change events for pagination.
   * @param event - The page change event containing the new page number.
   */
  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }
}
