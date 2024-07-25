import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

/**
 * Service to manage operations related to members, including fetching member data,
 * updating member information, and managing likes.
 */
@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  userParams: UserParams | undefined;
  user: User | undefined;

  /**
   * Constructor to inject HttpClient and AccountService.
   * Initializes userParams based on the current user.
   * @param http - HttpClient instance for making HTTP requests.
   * @param accountSvc - AccountService instance for managing user accounts.
   */
  constructor(private http: HttpClient, private accountSvc: AccountService) {
    this.accountSvc.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    });
  }

  /**
   * Retrieves the current user parameters.
   * @returns The current UserParams object.
   */
  getUserParams() {
    return this.userParams;
  }

  /**
   * Sets new user parameters.
   * @param params - New UserParams object to set.
   */
  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  /**
   * Resets user parameters to the default values based on the current user.
   * @returns The reset UserParams object, or `undefined` if no current user is set.
   */
  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }

  /**
   * Fetches a paginated list of members based on user parameters.
   * @param userParams - The parameters to filter and paginate the member list.
   * @returns An observable of a PaginatedResult containing an array of Members.
   */
  getMembers(userParams: UserParams) {
    let params = getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http);
  }

  /**
   * Fetches a single member by username.
   * @param username - The username of the member to retrieve.
   * @returns An observable of the Member object, or `of(member)` if the member is already in cache.
   */
  getMember(username: string) {
    // Uncomment and use cache logic if needed
    // const member = [...this.memberCache.values()]
    //   .reduce((arr, elem) => arr.concat(elem.result), [])
    //   .find((member: Member) => member.userName === username);

    const member = this.members.find((x) => x.userName == username);

    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  /**
   * Updates the details of a member.
   * @param member - The Member object with updated details.
   * @returns An observable that completes after updating the member.
   */
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = { ...this.members[index], ...member };
      })
    );
  }

  /**
   * Sets a member's photo as the main profile photo.
   * @param photoId - The ID of the photo to set as main.
   * @returns An observable that completes after setting the main photo.
   */
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  /**
   * Deletes a member's photo.
   * @param photoId - The ID of the photo to delete.
   * @returns An observable that completes after deleting the photo.
   */
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  /**
   * Adds a like for a member.
   * @param username - The username of the member to like.
   * @returns An observable that completes after adding the like.
   */
  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  /**
   * Retrieves a paginated list of members who have been liked.
   * @param predicate - The predicate to filter the liked members.
   * @param pageNumber - The page number to fetch.
   * @param pageSize - The number of items per page.
   * @returns An observable of a PaginatedResult containing an array of Members.
   */
  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);

    return getPaginatedResult<Member[]>(this.baseUrl + 'likes', params, this.http);
  }
}
