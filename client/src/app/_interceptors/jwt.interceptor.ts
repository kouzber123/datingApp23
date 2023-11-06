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

  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    //during http request we intercept and check current user thus take 1 and subscrip and auto set token
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user) {
          request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${user.token}`
            }
          })
        }
      }
    })
    return next.handle(request);
  }
}
