import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  @Output() cancelRegister = new EventEmitter();

  model:any = {};

  constructor(private accountSvc:AccountService, private toast:ToastrService) { }

  ngOnInit(): void {

  }

  /**
 * Function to initiate the registration process.
 * Calls the account service to register the user using the provided model.
 * Displays a toast notification with an error message if registration fails.
 */
register() {
  // Log the start of the registration process along with the model being used for registration.
  console.log("Register: ", this.model);

  // Call the register method of the account service and subscribe to the returned observable.
  this.accountSvc.register(this.model).subscribe({
    // Handle the next event emitted by the observable (successful registration response).
    next: () => {
      // If registration is successful, cancel the registration process.
      this.cancel();
    },
    // Handle the error event emitted by the observable (registration failure).
    error: error => {
      // Log the error received from the server.
      console.log(error);

      // Display a toast notification with the error message received from the server.
      this.toast.error(error.error);
    }
  });
}


 /**
 * Function to cancel the registration process.
 * Emits an event to indicate that the registration has been canceled.
 */
cancel() {
  // Prints a message in the console indicating that the registration has been canceled.
  console.log("Registration canceled");

  // Emits an event with the value 'false' to indicate that the registration has been canceled.
  this.cancelRegister.emit(false);
}


}
