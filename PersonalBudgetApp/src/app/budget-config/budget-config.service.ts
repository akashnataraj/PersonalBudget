import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, tap, take, map } from 'rxjs/operators';
import { throwError, BehaviorSubject, Subscription } from 'rxjs';

import { AuthService } from '../auth/auth.service';

export interface BudgetDTO {
  budgetId: string;
  userId: string;
  title: string;
  expense: number;
  budget: number;
  code: number;
  description: string;
  budgetList: Array<BudgetDTO>;
}

@Injectable()
export class BudgetService {
  budgetData = new BehaviorSubject<Array<BudgetDTO>>([]);

  constructor(private http: HttpClient, private authService: AuthService, private router: Router) { }

  getBudget(userId: string, date: string) {
    return this.http
      .get<BudgetDTO>(
        'http://localhost:53103/api/Budget/Get?userid=' + userId + '&date=' + date
      )
      .pipe(
        catchError(this.handleError),
        tap(resData => {
          if (resData.code === 200) {
            console.log(resData.budgetList);
          } else {
            throwError(resData.description);
          }
        })
      );
  }

  createBudget(userId: string, title: string, expense: number, budget: number, createdDate: Date) {
    console.log('service:' + createdDate);
    return this.http
      .post<BudgetDTO>(
        'http://localhost:53103/api/Budget/Create',
        {
          UserId: userId,
          Title: title,
          Expense: expense,
          Budget: budget,
          CreatedDate: createdDate
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

  updateBudget(budgetId: string, userId: string, title: string, expense: number, budget: number) {
    return this.http
      .put<BudgetDTO>(
        'http://localhost:53103/api/Budget/Update',
        {
          BudgetId: budgetId,
          UserId: userId,
          Title: title,
          Expense: expense,
          Budget: budget,
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

  deleteBudget(budgetId: string) {
    return this.http
      .delete<BudgetDTO>(
        'http://localhost:53103/api/Budget/Delete?budgetid=' + budgetId
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

  private handleError(errorRes: HttpErrorResponse) {
    const errorMessage = 'An unknown error occurred!';
    if (errorRes.error.description == null) {
      return throwError(errorMessage);
    } else {
      return throwError(errorRes.error.description);
    }
  }
}
