import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../_services/account.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountSvc: AccountService) {}

  // Intercepts all outgoing HTTP requests and attaches the JWT token if the user is authenticated
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    // Gets the current user from the account service
    this.accountSvc.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        // Checks if the user is authenticated
        if(user) {
          // Clones the request and attaches the authorization token in the headers
          request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${user.token}`
            }
          })
        }
      }
    })

    // Continues with handling the request
    return next.handle(request);
  }
}
