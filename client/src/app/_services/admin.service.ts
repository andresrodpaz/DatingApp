import { Injectable, OnInit } from '@angular/core';
import { User } from '../_models/user';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environment.apiUrl;

  constructor(private http : HttpClient) { }

  getUsersWithRoles(){
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  // updateUserRoles(username:string, roles:string){
  //   return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles,{});
  // }
  // updateUserRoles(username: string, roles: string[]) {
  //   const params = new HttpParams().set('roles', roles.join(','));
  //   return this.http.post<string[]>(`${this.baseUrl}admin/edit-roles/${username}`, {}, { params });
  // }
  updateUserRoles(username: string, roles: string[]): Observable<string[]> {
    if (roles.length === 0) {
      console.error('No roles selected');
      return throwError(() => new Error('No roles selected'));
    }

    const params = new HttpParams().set('roles', roles.join(','));

    console.log(`Sending request to: ${this.baseUrl}admin/edit-roles/${username}`);
    console.log(`With params: roles=${roles.join(',')}`);

    return this.http.post<string[]>(`${this.baseUrl}admin/edit-roles/${username}`, {}, { params }).pipe(
      catchError((error: any) => {
        console.error('Error occurred:', error);
        return throwError(() => error);
      })
    );
  }
}
