import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class authGuard implements CanActivate {

  constructor(
    private accountSvc: AccountService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  /**
   * Function to check if the user is authenticated and can activate the route.
   * Uses the currentUser$ observable from the account service to determine authentication status.
   * Displays a toast notification with an error message if the user is not authenticated.
   * Redirects to the login page if the user is not authenticated.
   * @param next The activated route snapshot.
   * @param state The router state snapshot.
   * @returns Observable<boolean> indicating whether the user is authenticated and can activate the route.
   */
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> {
      return this.accountSvc.currentUser$.pipe(
        map(user => {
          if (user) {
            return true;
          } else {
            this.toastr.error('You shall not pass!');
            this.router.navigate(['/login']); // Redirect to login page
            return false;
          }
        })
      );
  }
}
