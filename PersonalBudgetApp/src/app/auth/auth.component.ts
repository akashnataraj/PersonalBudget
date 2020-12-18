import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

import { AuthService, AccountDTO } from './auth.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent {
  isLoginMode = true;
  isLoading = false;
  error: string = null;

  constructor(private authService: AuthService, private router: Router) { }

  onSwitchMode() {
    this.isLoginMode = !this.isLoginMode;
  }

  onSubmit(form: NgForm) {
    if (!form.valid) {
      return;
    }

    const firstname = form.value.firstname;
    const lastname = form.value.lastname;
    const email = form.value.email;
    const password = form.value.password;

    let authObs: Observable<AccountDTO>;

    this.isLoading = true;

    if (this.isLoginMode) {
      authObs = this.authService.login(email, password, null);
    } else {
      authObs = this.authService.signup(firstname, lastname, email, password);
    }

    authObs.subscribe(
      resData => {
        console.log(resData);
        this.error = '';
        this.isLoading = false;
        if (resData.code == 200) {
          if (!this.isLoginMode) {
            alert('Registration successful, please login');
            this.isLoginMode = true;
          } else {
            this.router.navigate(['']);
          }
        } else {
          this.error = resData.description;
        }
      },
      errorMessage => {
        console.log(errorMessage);
        this.error = errorMessage;
        this.isLoading = false;
      }
    );

    form.reset();
  }
}
