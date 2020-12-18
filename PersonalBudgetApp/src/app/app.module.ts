import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';
import { AppRoutingModule } from './app-routing.module';
import { AuthComponent } from './auth/auth.component';
import { LoadingSpinnerComponent } from './shared/loading-spinner/loading-spinner.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { BudgetConfigComponent } from './budget-config/budget-config.component';
import { BudgetInterceptor } from './budget-config/budget-interceptor.service';
import { BudgetService } from './budget-config/budget-config.service';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    AuthComponent,
    LoadingSpinnerComponent,
    DashboardComponent,
    BudgetConfigComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: BudgetInterceptor, multi: true },
    BudgetService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
