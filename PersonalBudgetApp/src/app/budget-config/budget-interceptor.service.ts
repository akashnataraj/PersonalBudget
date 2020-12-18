import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable, Subscription } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Injectable()
export class BudgetInterceptor implements HttpInterceptor {

  userToken = '';
  private userSub: Subscription;

  constructor(private authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.url.search('/Account') == -1) {
      this.userSub = this.authService.user.subscribe(user => {
        this.userToken = user.token;
      });
      const mod_req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${this.userToken}`
        }
      });

      return next.handle(mod_req);
    }
    return next.handle(req);
  }

  ngOnDestroy() {
    this.userSub.unsubscribe();
  }
}
