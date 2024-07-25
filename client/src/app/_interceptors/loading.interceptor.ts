import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, delay, finalize, identity } from 'rxjs';
import { BusyService } from '../_services/busy.service';
import { environment } from 'src/environments/environment';

/**
 * Interceptor to manage a loading indicator during HTTP requests.
 * Shows a loading spinner at the start of the request and hides it upon completion.
 */
@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  /**
   * Constructor to inject the BusyService.
   * @param busySvc - Service to manage the loading indicator.
   */
  constructor(private busySvc: BusyService) {}

  /**
   * Intercepts HTTP requests to manage the loading indicator.
   * @param request - The outgoing HTTP request.
   * @param next - The HTTP handler to process the request.
   * @returns An observable of the HTTP event stream.
   */
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.busySvc.busy();

    return next.handle(request).pipe(
      (environment.production ? identity : delay(1000)), // Add delay in non-production environments
      finalize(() => {
        this.busySvc.idle();
      })
    );
  }
}
