import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  /**
   * Intercepts HTTP requests and handles errors globally.
   * @param request HttpRequest object representing the outgoing request
   * @param next HttpHandler for forwarding the request
   * @returns Observable<HttpEvent<unknown>>
   */
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) {
                const modelStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modelStateErrors.push(error.error.errors[key]);
                  }
                }
                // Throw the model state errors for handling at the caller end.
                throw modelStateErrors.flat();
              } else {
                // Display general error message using ToastrService.
                this.toastr.error(error.error, error.status.toString());
              }
              // Redirect to the 'bad-request' page using Router service.
              this.router.navigateByUrl('/bad-request');
              break;
            case 401:
              // Display 'Unauthorized' error message using ToastrService.
              this.toastr.error('Unauthorized', error.status.toString());
              break;
            case 404:
              // Redirect to the 'not-found' page using Router service.
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              // Redirect to the 'server-error' page with error details using Router service.
              const navigationExtras: NavigationExtras = { state: { error: error.error } };
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              // Display generic error message using ToastrService and log the error.
              this.toastr.error('Something unexpected went wrong!');
              console.log(error);
              break;
          }
        }
        // Rethrow the error to propagate it down the chain.
        return throwError(error);
      })
    );
  }
}
