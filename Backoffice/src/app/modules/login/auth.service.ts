import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginModel } from './model/login.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
 constructor(private http: HttpClient) { }

  login(email: string, password: string): Observable<LoginModel> {


    return this.http.post<LoginModel>(
      `/api/identity/login`,
      { email, password },
      { withCredentials: true }
    );
  }
}
