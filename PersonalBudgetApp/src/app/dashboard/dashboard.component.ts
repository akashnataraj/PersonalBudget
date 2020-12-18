import { Component, OnDestroy } from '@angular/core';
import { BudgetDTO, BudgetService } from '../budget-config/budget-config.service';
import { Router } from '@angular/router';
import { Chart } from 'chart.js';
import { Subscription } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnDestroy {
  private userSub: Subscription;
  private budgetSub: Subscription;
  budgetList: Array<BudgetDTO>;
  maxYear = new Date().getFullYear() + 1;
  userId: string;
  month: number;
  year: number;
  error: string = null;

  public dataSource = {
    labels: [],
    datasets: [
      {
        label: 'Budget',
        data: [],
        backgroundColor: '#ffcd56'
      },
      {
        label: 'Expense',
        data: [],
        backgroundColor: '#ff6384'
      }
    ]
  };

  public pieDataSource = {
    labels: [],
    datasets: [
      {
        label: 'Budget',
        data: [],
        backgroundColor: [
          '#ffcd56',
          '#ff6384',
          '#36a2eb',
          '#fd6b19',
          '#0cda85',
          '#7748d2',
          '#ff0000',
          '#42ccc0',
          '#5046e7'
        ]
      }
    ]
  };

  constructor(private authService: AuthService, private budgetService: BudgetService, private router: Router) {
    this.month = new Date().getMonth() + 1;
    this.year = new Date().getFullYear();
    this.fetchBudget();
  }

  fetchBudget() {
    this.authService.user.subscribe(user => {
      this.userId = user.id;
    });
    this.budgetService.getBudget(this.userId, new Date(this.year, this.month - 1).toLocaleString()).subscribe(budget => {
      this.budgetList = budget.budgetList;
      console.log('date' + this.month + this.year);
      this.createChart();
    });
  }

  createChart() {
    this.dataSource.datasets[0].data = [];
    this.dataSource.datasets[1].data = [];
    this.dataSource.labels = [];
    this.pieDataSource.datasets[0].data = [];
    this.pieDataSource.labels = [];
    for (let i = 0; i < this.budgetList.length; i++) {
      this.dataSource.datasets[0].data[i] = this.budgetList[i].budget;
      this.dataSource.datasets[1].data[i] = this.budgetList[i].expense;
      this.dataSource.labels[i] = this.budgetList[i].title;
      this.pieDataSource.datasets[0].data[i] = this.budgetList[i].budget;
      this.pieDataSource.labels[i] = this.budgetList[i].title;
    }
    const ctx3 = document.getElementById('lineChart');
    const lineChart = new Chart(ctx3, {
      type: 'line',
      data: this.dataSource
    });

    const ctx2 = document.getElementById('barChart');
    const barChart = new Chart(ctx2, {
      type: 'bar',
      data: this.dataSource
    });

    const ctx1 = document.getElementById('pieChart');
    const myPieChart = new Chart(ctx1, {
      type: 'pie',
      data: this.pieDataSource
    });
  }

  ngOnDestroy() {
    //this.userSub.unsubscribe();
    //this.budgetSub.unsubscribe();
  }
}
