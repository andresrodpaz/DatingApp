import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, catchError, throwError } from 'rxjs';

/**
 * Service to handle administrative operations related to users.
 * This includes fetching users with their roles and updating user roles.
 */
@Injectable({
  providedIn: 'root'
})
export class AdminService {

  /**
   * The base URL for API requests, derived from environment configuration.
   */
  baseUrl = environment.apiUrl;

  /**
   * Constructor to inject the HttpClient.
   * @param http - The HttpClient for making HTTP requests.
   */
  constructor(private http: HttpClient) { }

  /**
   * Fetches users along with their roles.
   * @returns An observable of an array of users.
   */
  getUsersWithRoles(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  /**
   * Updates the roles for a specific user.
   * @param username - The username of the user whose roles are to be updated.
   * @param roles - The new roles to be assigned to the user.
   * @returns An observable of an array of strings representing the updated roles.
   */
  updateUserRoles(username: string, roles: string[]): Observable<string[]> {
    // Check if roles array is empty
    if (roles.length === 0) {
      console.error('No roles selected');
      return throwError(() => new Error('No roles selected'));
    }

    // Create HttpParams with roles joined by commas
    const params = new HttpParams().set('roles', roles.join(','));

    // Log the request details
    console.log(`Sending request to: ${this.baseUrl}admin/edit-roles/${username}`);
    console.log(`With params: roles=${roles.join(',')}`);

    // Make HTTP POST request to update roles
    return this.http.post<string[]>(`${this.baseUrl}admin/edit-roles/${username}`, {}, { params }).pipe(
      // Catch and handle errors
      catchError((error: any) => {
        console.error('Error occurred:', error);
        return throwError(() => error);
      })
    );
  }
}
