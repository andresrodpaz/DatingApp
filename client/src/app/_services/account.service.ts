import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';

import { jwtDecode } from 'jwt-decode';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  // Base URL for API requests
  baseUrl = environment.apiUrl;

  // BehaviorSubject to hold the current user information
  private currentUserSource = new BehaviorSubject<User | null>(null);


  // Observable to subscribe to for changes in the current user
  currentUser$: Observable<User | null> = this.currentUserSource.asObservable();

  constructor(private http:HttpClient, private presenceService : PresenceService) {

  }

  /**
   * Function to perform user login.
   * @param model Object containing login credentials.
   * @returns Observable of User object.
   */
  login(model: any): Observable<boolean> {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        // On successful login response
        const user = response;
        if (user) {
          // Save user data to local storage
          localStorage.setItem('user', JSON.stringify(user));
          // Update current user source with new user data
          this.currentUserSource.next(user);
          // Emit true to indicate successful login
          return true;
        } else {
          // Emit false if user is null or undefined
          return false;
        }
      })
    );
  }



  register(model:any){
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map( user => {
        if(user){

          this.setCurrentUser(user);
        }

      }

      )
    )
  }

  /**
   * Function to set the current user.
   * @param user User object to set as the current user.
   */
  setCurrentUser(user: User) {
    user.roles = []; // Inicializamos o reiniciamos el array de roles

    // Obtenemos los roles del token decodificado
    const roles = this.getDecodedToken(user.token)?.role;

    // Verificamos si roles es un array y lo asignamos, o si no, lo agregamos al array de roles
    if (Array.isArray(roles)) {
      user.roles = roles;
    } else if (roles) {
      user.roles.push(roles);
    }

    // Almacenamos el usuario en localStorage
    localStorage.setItem('user', JSON.stringify(user));

    // Emitimos el usuario actualizado al BehaviorSubject
    this.currentUserSource.next(user);

    // Creamos la conexión con SignalR utilizando el servicio de presencia
    this.presenceService.createHubConnection(user);
  }


  /**
   * Function to logout the current user.
   * Removes user data from local storage.
   */
  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null); // Notifica a los componentes que el usuario ha cerrado sesión
    this.presenceService.stopHubConnection();
  }

  getDecodedToken(token:string){
    return JSON.parse(atob(token.split('.')[1]))
  }

}
