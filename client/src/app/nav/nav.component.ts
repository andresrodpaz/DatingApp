import { Component, OnInit } from '@angular/core';
import { NgModel } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};
  currentUser$ : Observable<User | null > = of(null);

  isNavbarCollapsed = true;
  constructor(public accountSvc: AccountService,private router:Router, private toastr:ToastrService) {}
  ngOnInit(): void {
    this.currentUser$ = this.accountSvc.currentUser$;
  }


  /**
   * Initiates the login process.
   * Logs the user credentials and attempts to log in using the account service.
   * Handles the response from the login attempt.
   */
  login() {
    // Log the start of the login process.
    console.log('Login');

    // Log the user credentials being used for login.
    console.log(this.model);

    // Call the login method of the account service and subscribe to the returned observable.
    this.accountSvc.login(this.model).subscribe({
      // Handle the next event emitted by the observable (successful login response).
      next: (response) => {
        // Log that the login was successful.
        console.log('Login OK');
        this.router.navigateByUrl('/members')
        // Log the response received from the server.
        console.log(response);
      },
      // Handle the error event emitted by the observable (login failure).
      error: (error) => {
        // Log that the login was not successful.
        console.log('Login NO OK');
        this.toastr.error(error.error);
        // Log the error received from the server.
        console.log(error);
      },
    });
  }


  logout() {
    this.accountSvc.logout();
    this.router.navigateByUrl('/');
  }

  // Método para manejar el estado del menú hamburguesa
  toggleNavbar() {
    this.isNavbarCollapsed = !this.isNavbarCollapsed; // Invierte el estado del menú
  }

  // Método para cerrar el menú después de hacer clic en un enlace
  closeNavbar() {
    this.isNavbarCollapsed = true;
  }
}
