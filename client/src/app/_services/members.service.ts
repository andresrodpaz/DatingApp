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

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  //memberCache = new Map();
  userParams: UserParams | undefined;
  user: User | undefined;

  constructor(private http: HttpClient, private accountSvc:AccountService) {
    this.accountSvc.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    });
  }
  getUserParams(){
    return this.userParams;
  }
  setUserParams(params : UserParams){
    this.userParams = params;
  }
  resetUserParams(){
    if(this.user){
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }

  getMembers(userPararms: UserParams) {
    let params = getPaginationHeaders(
      userPararms.pageNumber,
      userPararms.pageSize
    );

    params = params.append('minAge', userPararms.minAge);
    params = params.append('maxAge', userPararms.maxAge);
    params = params.append('gender', userPararms.gender);
    params = params.append('orderBy', userPararms.orderBy)

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http);
  }


  getMember(username: string) {
    // const member =[...this.memberCache.values()]
    // .reduce((arr, elem) => arr.concat(elem.result), [])
    // .find((member:Member)=> member.userName === username);
    const member = this.members.find((x) => x.userName == username);

    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = { ...this.members[index], ...member };
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addLike(username:string){
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }
  getLikes(predicate:string, pageNumber:number, pageSize:number ){

    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);

    return getPaginatedResult<Member[]>(this.baseUrl + 'likes' , params, this.http );
  }
}
