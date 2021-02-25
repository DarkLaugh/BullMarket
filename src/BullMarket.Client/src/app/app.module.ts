import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { LoginComponent } from './components/auth/login/login.component';
import { AuthService } from 'src/app/services/auth/auth.service'; 
import { AuthGuard } from 'src/app/services/auth/auth-guard.service'; 
import { AuthInterceptor } from 'src/app/services/auth/auth-interceptor.service'; 

import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { JwtModule } from '@auth0/angular-jwt';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    PasswordModule,
    InputTextModule,
    ButtonModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: () => localStorage.getItem('token')
      }
    })
  ],
  providers: [
    AuthService,
    AuthGuard,
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
