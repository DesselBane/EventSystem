import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {AuthenticationService} from './authentication.service';

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {


  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Get the auth header from the service.
    let header = req.headers.set('Content-Type', 'application/json');
    if (AuthenticationService.isAuthenticated()) {
      header = header.set('Authorization', `Bearer ${AuthenticationService.loginToken.accessToken}`);
    }
    // Clone the request to add the new header.
    // Pass on the cloned request instead of the original request.
    return next.handle(req.clone({headers: header}));
  }
}
