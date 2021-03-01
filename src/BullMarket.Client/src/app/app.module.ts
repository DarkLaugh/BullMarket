import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { LoginComponent } from './components/auth/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { AuthService } from 'src/app/services/auth/auth.service'; 
import { StockRestService } from 'src/app/services/rest/stock-rest.service';
import { StockRealTimeService } from 'src/app/services/real-time/stock-real-time.service';
import { AuthGuard } from 'src/app/services/auth/auth-guard.service'; 
import { AuthInterceptor } from 'src/app/services/auth/auth-interceptor.service'; 

import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { JwtModule } from '@auth0/angular-jwt';
import { StockComponent } from './components/stock/stock.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    StockComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    PasswordModule,
    InputTextModule,
    ButtonModule,
    TableModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: () => localStorage.getItem('token')
      }
    })
  ],
  providers: [
    AuthService,
    AuthGuard,
    StockRestService,
    StockRealTimeService,
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
