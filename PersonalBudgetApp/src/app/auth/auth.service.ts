import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, tap } from 'rxjs/operators';
import { throwError, BehaviorSubject } from 'rxjs';
import { User } from './user.model';

export interface AccountDTO {
  userID: string;
  firstName: string;
  lastName: string;
  username: string;
  password: string;
  token: string;
  refreshToken: string;
  expiresIn: number;
  code: number;
  description: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  user = new BehaviorSubject<User>(null);
  private tokenExpirationTimer: any;
  private tokenExpireWarning: any;

  constructor(private http: HttpClient, private router: Router) { }

  signup(firstname: string, lastname: string, email: string, password: string) {
    return this.http
      .post<AccountDTO>(
        'http://localhost:53103/api/Account/Register',
        {
          FirstName: firstname,
          LastName: lastname,
          UserName: email,
          Password: password,
        }
      )
      .pipe(
        catchError(this.handleError),
        tap(resData => {
          if (resData.code! == 200) {
            throwError(resData.description);
          }
        })
      );
  }

  login(email: string, password: string, refreshToken: string) {
    console.log(refreshToken);
    return this.http
      .post<AccountDTO>(
        'http://localhost:53103/api/Account/Login',
        {
          Username: email,
          Password: password,
          RefreshToken: refreshToken
        }
      )
      .pipe(
        catchError(this.handleError),
        tap(resData => {
          this.handleAuthentication(
            resData.userID,
            resData.firstName,
            resData.lastName,
            resData.username,
            resData.password,
            resData.token,
            resData.refreshToken,
            resData.expiresIn,
            resData.code,
            resData.description
          );
        })
      );
  }

  autoLogin() {
    const userData: {
      email: string;
      id: string;
      _token: string;
      _refreshToken: string;
      _tokenExpirationDate: string;
    } = JSON.parse(localStorage.getItem('userData'));
    if (!userData) {
      return;
    }

    const loadedUser = new User(
      userData.email,
      userData.id,
      userData._token,
      userData._refreshToken,
      new Date(userData._tokenExpirationDate)
    );

    if (loadedUser.token) {
      this.user.next(loadedUser);
      const expirationDuration =
        new Date(userData._tokenExpirationDate).getTime() -
        new Date().getTime();
      this.autoLogout(expirationDuration);
    }
  }

  logout() {
    this.user.next(null);
    this.router.navigate(['/login']);
    localStorage.removeItem('userData');
    if (this.tokenExpirationTimer) {
      clearTimeout(this.tokenExpirationTimer);
    }
    this.tokenExpirationTimer = null;
    this.tokenExpireWarning = null;
  }

  autoLogout(expirationDuration: number) {
    this.tokenExpireWarning = setTimeout(() => {
      if (confirm('Session will expire in 20 seconds, do you wish to continue?')) {
        clearTimeout(this.tokenExpirationTimer);
        clearTimeout(this.tokenExpireWarning);
        const userData: {
          email: string;
          id: string;
          _token: string;
          _refreshToken: string;
          _tokenExpirationDate: string;
        } = JSON.parse(localStorage.getItem('userData'));
        this.login(userData.email, null, userData._refreshToken).subscribe(user => {
        });
      }
    }, expirationDuration - 20000);
    this.tokenExpirationTimer = setTimeout(() => {
      this.logout();
    }, expirationDuration);
  }

  private handleAuthentication(
    UserID: string,
    FirstName: string,
    LastName: string,
    Username: string,
    Password: string,
    Token: string,
    RefreshToken: string,
    ExpiresIn: number,
    code: number,
    description: string,
  ) {
    if (code == 200) {
      const expirationDate = new Date(new Date().getTime() + ExpiresIn * 1000);
      const user = new User(Username, UserID, Token, RefreshToken, expirationDate);
      this.user.next(user);
      this.autoLogout(ExpiresIn * 1000);
      localStorage.setItem('userData', JSON.stringify(user));
      console.log('success:' + user);
    } else {
      console.log(description);
      return throwError(description);
    }
  }

  private handleError(errorRes: HttpErrorResponse) {
    const errorMessage = 'An unknown error occurred!';
    if (errorRes.error.description == null) {
      return throwError(errorMessage);
    } else {
      return throwError(errorRes.error.description);
    }
  }
}
