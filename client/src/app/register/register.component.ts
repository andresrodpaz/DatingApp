import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  @Output() cancelRegister = new EventEmitter();

  model:any = {};
  registerForm : FormGroup = new FormGroup({});
  validationErrors:string[] | undefined;

  constructor(private accountSvc:AccountService, private toast:ToastrService, private fb:FormBuilder, private router:Router) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  /**
 * Function to initiate the registration process.
 * Calls the account service to register the user using the provided model.
 * Displays a toast notification with an error message if registration fails.
 */
register() {
  // Log the start of the registration process along with the model being used for registration.
  const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value)
  const values = {...this.registerForm.value, dateOfBirth: dob};

  //Call the register method of the account service and subscribe to the returned observable.
  this.accountSvc.register(values).subscribe({
    // Handle the next event emitted by the observable (successful registration response).
    next: () => {
      // If registration is successful, redirects to the members section
      this.router.navigateByUrl('/members')
    },
    // Handle the error event emitted by the observable (registration failure).
    error: error => {
      // Log the error received from the server.
      this.validationErrors = error;

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

initializeForm(){
  this.registerForm = this.fb.group({
    gender: ['male', Validators.required],
    username: ['', Validators.required],
    knownAs: ['', Validators.required],
    dateOfBirth: ['', [Validators.required, this.validateDOB]],
    city: ['', Validators.required],
    country: ['', Validators.required],
    password: ['', [
      Validators.minLength(4), Validators.maxLength(20)
    ]],
    confirmPassword:  ['', [Validators.required, this.matchValues('password')]],
  });

  this.registerForm.controls['password'].valueChanges.subscribe({
    next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
  })
}

matchValues(matchTo:string):ValidatorFn{
  return (control : AbstractControl) => {
    return control.value === control.parent?.get(matchTo)?.value ? null : {notMaching : true}
  }
}

// Función para validar que la fecha de nacimiento asegure al menos 18 años
validateDOB(control: FormControl): { [key: string]: any } | null {
  if (control.value) {
    const selectedDate = new Date(control.value);
    const today = new Date();
    const minDate = new Date(today.getFullYear() - 18, today.getMonth(), today.getDate());

    if (selectedDate > minDate) {
      return { invalidDOB: true };
    }
  }
  return null;
}

private getDateOnly(dob: string | undefined){
  if(!dob) return;

  let theDob = new Date(dob);
  return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).toISOString().slice(0,10);
}
}
