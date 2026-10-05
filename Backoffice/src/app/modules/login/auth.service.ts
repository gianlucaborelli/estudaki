import { inject, Injectable, signal, makeStateKey, TransferState, PLATFORM_ID } from '@angular/core';
import { isPlatformServer } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of, tap, map, catchError, shareReplay } from 'rxjs';
import { LoginModel, UserMeModel } from './model/login.model';

const AUTH_USER_KEY = makeStateKey<UserMeModel | null>('auth_user');

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private transferState = inject(TransferState);
  private platformId = inject(PLATFORM_ID);

  private _currentUser = signal<UserMeModel | null>(null);
  public currentUser = this._currentUser.asReadonly();

  private ongoingCheck$?: Observable<boolean>;

  constructor() {
    if (this.transferState.hasKey(AUTH_USER_KEY)) {
      const user = this.transferState.get(AUTH_USER_KEY, null);
      this._currentUser.set(user);
    }
  }

  isAuthenticated(): boolean {
    return !!this._currentUser()?.isAuthenticated;
  }

  /**
   * Verifica a autenticação consultando o endpoint /me no backend.
   * Utiliza TransferState para sincronizar o estado SSR -> Browser sem chamadas duplicadas na hidratação.
   */
  checkAuth(): Observable<boolean> {
    if (this.transferState.hasKey(AUTH_USER_KEY)) {
      const user = this.transferState.get(AUTH_USER_KEY, null);
      this.transferState.remove(AUTH_USER_KEY);
      this._currentUser.set(user);
      return of(!!user?.isAuthenticated);
    }

    if (this._currentUser()) {
      return of(this.isAuthenticated());
    }

    if (this.ongoingCheck$) {
      return this.ongoingCheck$;
    }

    this.ongoingCheck$ = this.http.get<UserMeModel>('/api/identity/me').pipe(
      map((user) => {
        this._currentUser.set(user);
        if (isPlatformServer(this.platformId)) {
          this.transferState.set(AUTH_USER_KEY, user);
        }
        return !!user?.isAuthenticated;
      }),
      catchError(() => {
        this._currentUser.set(null);
        if (isPlatformServer(this.platformId)) {
          this.transferState.set(AUTH_USER_KEY, null);
        }
        return of(false);
      }),
      shareReplay(1),
      tap(() => {
        this.ongoingCheck$ = undefined;
      })
    );

    return this.ongoingCheck$;
  }

  login(email: string, password: string): Observable<LoginModel> {
    return this.http
      .post<LoginModel>(
        `/api/identity/login`,
        { email, password },
        { withCredentials: true }
      )
      .pipe(
        tap(() => {
          this._currentUser.set(null);
          this.checkAuth().subscribe();
        })
      );
  }

  logout(): void {
    this._currentUser.set(null);
  }
}
