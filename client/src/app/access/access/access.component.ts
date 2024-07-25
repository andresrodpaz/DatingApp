import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidatorFn, FormControl } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-access',
  templateUrl: './access.component.html',
  styleUrls: ['./access.component.css']
})
export class AccessComponent implements OnInit {
  model: any = {};
  modelRegister: any = {};
  modelLogin: any = {};
  currentUser$: Observable<User | null> = of(null);
  registerForm: FormGroup = new FormGroup({});
  loginForm: FormGroup = new FormGroup({});
  validationErrors: string[] | undefined;
  passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,20}$/;

  passwordVisible = false;
  confirmPasswordVisible = false;

  /**
   * Initializes a new instance of the AccessComponent.
   * @param accountSvc - The AccountService for user authentication and registration.
   * @param toast - The ToastrService for displaying notifications.
   * @param fb - The FormBuilder for creating form groups.
   * @param router - The Router for navigation.
   */
  constructor(
    private accountSvc: AccountService,
    private toast: ToastrService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  /**
   * Angular lifecycle hook that initializes the component.
   */
  ngOnInit(): void {
    this.currentUser$ = this.accountSvc.currentUser$;
    this.initializeForms();
    this.setupToggle();
  }

  /**
   * Initializes the registration and login forms with validators.
   */
  private initializeForms() {
    this.registerForm = this.fb.group({
      gender: ['male', Validators.required],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', [Validators.required, this.validateDOB]],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(20),
        Validators.pattern(this.passwordPattern)
      ]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
    });

    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });

    // Update confirmPassword validity when password changes
    this.registerForm.controls['password'].valueChanges.subscribe(() => {
      this.registerForm.controls['confirmPassword'].updateValueAndValidity();
    });
  }

  /**
   * Sets up event listeners for form toggle functionality.
   */
  private setupToggle() {
    const container = document.getElementById('container');
    const registerBtn = document.getElementById('register');
    const loginBtn = document.getElementById('login');

    registerBtn?.addEventListener('click', () => {
      container?.classList.add('active');
    });

    loginBtn?.addEventListener('click', () => {
      container?.classList.remove('active');
    });
  }

  /**
   * Registers a new user by submitting the registration form.
   */
  register() {
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = { ...this.registerForm.value, dateOfBirth: dob };

    this.accountSvc.register(values).subscribe({
      next: () => this.router.navigateByUrl('/members'),
      error: error => {
        this.validationErrors = error;
        this.toast.error(error.error);
      }
    });
  }

  /**
   * Logs in a user by submitting the login form.
   */
  login() {
    console.log(this.modelLogin);
    console.log(this.loginForm.value);
    this.accountSvc.login(this.modelLogin).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
      },
      error: error => {
        this.toast.error(error.error);
      }
    });
  }

  /**
   * Cancels the registration process and logs a message.
   */
  cancel() {
    console.log('Registration canceled');
  }

  /**
   * Validator function to ensure the selected date of birth is at least 18 years in the past.
   * @param control - The form control containing the date of birth value.
   * @returns An error object if validation fails, otherwise null.
   */
  private validateDOB(control: FormControl): { [key: string]: any } | null {
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

  /**
   * Validator function to ensure the value of one control matches another control's value.
   * @param matchTo - The name of the control to match.
   * @returns A validator function that returns an error object if values do not match.
   */
  private matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : { notMatching: true };
    };
  }

  /**
   * Formats the date of birth to 'YYYY-MM-DD'.
   * @param dob - The date of birth value.
   * @returns The formatted date of birth string.
   */
  private getDateOnly(dob: string | undefined) {
    if (!dob) return;

    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset())).toISOString().slice(0, 10);
  }

  /**
   * Toggles the visibility of the password field.
   */
  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }

  /**
   * Toggles the visibility of the confirm password field.
   */
  toggleConfirmPasswordVisibility() {
    this.confirmPasswordVisible = !this.confirmPasswordVisible;
  }
}
