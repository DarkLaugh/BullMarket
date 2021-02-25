import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LoginModel } from 'src/app/models/auth/login.model';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ApiPaths } from 'src/app/constants/api.constants';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private username: string;
  private userId: number;

  constructor(
    private http: HttpClient,
    private jwtService: JwtHelperService,
  ) { }

  login(model: LoginModel): Observable<any> {
    let url = environment.backEndUrl + ApiPaths.Login;
    return this.http.post<any>(environment.backEndUrl + ApiPaths.Login, model);
  }

  loggedIn() {
    let token = this.getToken();

    return token && !this.jwtService.isTokenExpired(token);
  }

  saveToken(token) {
    localStorage.setItem('token', token);
  }

  getToken() {
    return localStorage.getItem('token');
  }

  getId(): number {
    if(this.userId)
      return this.userId;

    let token = this.getToken();

    let decoded = this.jwtService.decodeToken(token);
    this.userId = +decoded.id;

    return this.userId;
  }
}
