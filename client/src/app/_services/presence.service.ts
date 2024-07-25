import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  /**
   * Initializes a new instance of the PresenceService.
   * @param toastr - The ToastrService for showing notifications.
   * @param router - The Router for navigation.
   */
  constructor(private toastr: ToastrService, private router: Router) { }

  /**
   * Creates and starts a SignalR hub connection.
   * @param user - The user object containing the token for authentication.
   */
  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    // Start the connection
    this.hubConnection.start().catch(error => console.error('Error starting SignalR connection:', error));

    // Handle the 'UserIsOnline' event
    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUsersSource.next([...usernames, username])
      });
      // Optional: Show a notification when a user comes online
      // this.toastr.info(username + ' has connected');
    });

    // Handle the 'UserIsOffline' event
    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUsersSource.next(usernames.filter(x => x !== username))
      });
      // Optional: Show a notification when a user goes offline
      // this.toastr.warning(username + ' has disconnected');
    });

    // Handle the 'GetOnlineUsers' event
    this.hubConnection.on('GetOnlineUsers', usernames => {
      this.onlineUsersSource.next(usernames);
    });

    // Handle the 'NewMessageReceived' event
    this.hubConnection.on('NewMessageReceived', ({ username, knownAs }) => {
      this.toastr.info(`📬 ${knownAs} has sent you a new message! Tap here to check it out!`)
        .onTap
        .pipe(take(1))
        .subscribe({
          next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')
        });
    });
  }

  /**
   * Stops the SignalR hub connection.
   */
  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.error('Error stopping SignalR connection:', error));
  }
}
