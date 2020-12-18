import { Component, OnInit, OnDestroy } from '@angular/core';
import { BudgetDTO, BudgetService } from './budget-config.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-budget-config',
  templateUrl: './budget-config.component.html',
  styleUrls: ['./budget-config.component.css']
})
export class BudgetConfigComponent implements OnDestroy {
  private userSub: Subscription;
  private budgetSub: Subscription;
  budgetList: Array<BudgetDTO>;
  maxYear = new Date().getFullYear() + 1;
  userId: string;
  budgetId: string;
  isAdd = true;
  category: string;
  budget: number;
  expense: number;
  month: number;
  year: number;
  error: string = null;

  constructor(private authService: AuthService, private budgetService: BudgetService, private router: Router) {
    this.month = new Date().getMonth() + 1;
    this.year = new Date().getFullYear();
    this.fetchBudget();
  }

  fetchBudget() {
    this.userSub = this.authService.user.subscribe(user => {
      this.userId = user.id;
    });
    this.budgetSub = this.budgetService.getBudget(this.userId, new Date(this.year, this.month - 1).toLocaleString()).subscribe(budget => {
      this.budgetList = budget.budgetList;
      console.log(this.budgetList);
    });
  }

  onEdit(budgetId: string, title: string, budget: number, expense: number) {
    this.isAdd = false;
    this.budgetId = budgetId;
    this.category = title;
    this.budget = budget;
    this.expense = expense;
  }

  switchToAdd() {
    this.isAdd = true;
    this.category = '';
    this.budget = null;
    this.expense = null;
  }

  onDelete(budgetId: string) {
    if (confirm('Are you sure to delete?')) {
      this.budgetSub = this.budgetService.deleteBudget(budgetId).subscribe(budget => {
        if (budget.code >= 200 && budget.code < 210) {
          alert('Delete successful');
          this.isAdd = true;
          this.fetchBudget();
        } else {
          this.error = budget.description;
        }
      },
        errorMessage => {
          console.log(errorMessage);
        });
    }
  }

  onSubmit(form: NgForm) {
    if (!form.valid) {
      return;
    }
    console.log('this.add:' + this.isAdd);
    if (this.isAdd) {
      this.budgetSub = this.budgetService.createBudget(this.userId, form.value.category, form.value.expense, form.value.budget, new Date(this.year, this.month - 1)).subscribe(budget => {
        if (budget.code >= 200 && budget.code < 210) {
          alert('Create successful');
          this.isAdd = true;
          this.fetchBudget();
        } else {
          this.error = budget.description;
        }
      },
        errorMessage => {
          console.log(errorMessage);
        });
    } else {
      this.budgetSub = this.budgetService.updateBudget(this.budgetId, this.userId, form.value.category, form.value.expense, form.value.budget).subscribe(budget => {
        if (budget.code >= 200 && budget.code < 210) {
          alert('Update successful');
          this.isAdd = true;
          this.fetchBudget();
        } else {
          this.error = budget.description;
        }
      },
        errorMessage => {
          console.log(errorMessage);
        });
    }
    form.reset();
  }

  ngOnDestroy() {
    this.userSub.unsubscribe();
    this.budgetSub.unsubscribe();
  }
}
