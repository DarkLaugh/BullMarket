import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LoginModel } from 'src/app/models/auth/login.model';
import { BehaviorSubject, Observable, combineLatest } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ApiPaths } from 'src/app/constants/api.constants';
import {
  LoginOptions,
  OAuthErrorEvent,
  OAuthService,
  TokenResponse,
} from 'angular-oauth2-oidc';
import { filter, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private username: string;
  private userId: number;

  private isAuthenticatedSubject$ = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject$.asObservable();

  private isDoneLoadingSubject$ = new BehaviorSubject<boolean>(false);
  public isDoneLoading$ = this.isDoneLoadingSubject$.asObservable();

  /**
   * Publishes `true` if and only if (a) all the asynchronous initial
   * login calls have completed or errorred, and (b) the user ended up
   * being authenticated.
   *
   * In essence, it combines:
   *
   * - the latest known state of whether the user is authorized
   * - whether the ajax calls for initial log in have all been done
   */
  public canActivateProtectedRoutes$: Observable<boolean> = combineLatest([
    this.isAuthenticated$,
    this.isDoneLoading$,
  ]).pipe(map((values) => values.every((b) => b)));

  private navigateToLoginPage() {
    // TODO: Remember current URL
    this.router.navigate(['login']);
  }

  constructor(
    private http: HttpClient,
    private jwtService: JwtHelperService,
    private oauthService: OAuthService,
    private router: Router
  ) {
    // Useful for debugging:
    // this.oauthService.events.subscribe((event) => {
    //   if (event instanceof OAuthErrorEvent) {
    //     console.error('OAuthErrorEvent Object:', event);
    //   } else {
    //     console.warn('OAuthEvent Object:', event);
    //   }
    // });

    this.oauthService.events.subscribe((_) => {
      this.isAuthenticatedSubject$.next(
        this.oauthService.hasValidAccessToken()
      );
    });
    this.isAuthenticatedSubject$.next(this.oauthService.hasValidAccessToken());

    this.oauthService.events
      .pipe(filter((e) => ['token_received'].includes(e.type)))
      .subscribe((e) => this.oauthService.loadUserProfile());

    this.oauthService.events
      .pipe(
        filter((e) => ['session_terminated', 'session_error'].includes(e.type))
      )
      .subscribe((e) => this.navigateToLoginPage());

    this.oauthService.setupAutomaticSilentRefresh();
  }

  public runInitialLoginSequence(): Promise<void> {
    if (location.hash) {
      console.log('Encountered hash fragment, plotting as table...');
      console.table(
        location.hash
          .substring(1)
          .split('&')
          .map((kvp) => kvp.split('='))
      );
    }

    // 0. LOAD CONFIG:
    // First we have to check to see how the IdServer is
    // currently configured:
    return (
      this.oauthService
        // 1. HASH LOGIN:
        // Try to log in via hash fragment after redirect back
        // from IdServer from initImplicitFlow:
        .loadDiscoveryDocument()
        .then(() => this.oauthService.tryLoginCodeFlow())

        .then(() => {
          if (this.oauthService.hasValidAccessToken()) {
            return Promise.resolve();
          }

          // 2. SILENT LOGIN:
          // Try to log in via a refresh because then we can prevent
          // needing to redirect the user:
          if (this.oauthService.getRefreshToken()) {
            console.log('Trying silent login via refresh...');
            return this.oauthService
              .refreshToken()
              .then(() => {
                Promise.resolve();
              })
              .catch((result) => {
                // Subset of situations from https://openid.net/specs/openid-connect-core-1_0.html#AuthError
                // Only the ones where it's reasonably sure that sending the
                // user to the IdServer will help.
                const errorResponsesRequiringUserInteraction = [
                  'interaction_required',
                  'login_required',
                  'account_selection_required',
                  'consent_required',
                ];

                if (
                  result &&
                  result.reason &&
                  errorResponsesRequiringUserInteraction.indexOf(
                    result.reason.error
                  ) >= 0
                ) {
                  // 3. ASK FOR LOGIN:
                  // At this point we know for sure that we have to ask the
                  // user to log in, so we redirect them to the IdServer to
                  // enter credentials.
                  //
                  // Enable this to ALWAYS force a user to login.
                  // this.loginViaKeycloak();
                  //
                  // Instead, we'll now do this:
                  console.warn(
                    'User interaction is needed to log in, we will wait for the user to manually log in.'
                  );
                  return Promise.resolve();
                }

                console.warn('Error while silently logging in. Moving on...');
                // We can't handle the truth, just pass on the problem to the
                // next handler.
                return Promise.reject(result);
              });
          }
        })

        .then(() => {
          this.isDoneLoadingSubject$.next(true);

          // Check for the strings 'undefined' and 'null' just to be sure. Our current
          // login(...) should never have this, but in case someone ever calls
          // initImplicitFlow(undefined | null) this could happen.
          if (
            this.oauthService.state &&
            this.oauthService.state !== 'undefined' &&
            this.oauthService.state !== 'null'
          ) {
            let stateUrl = this.oauthService.state;
            if (stateUrl.startsWith('/') === false) {
              stateUrl = decodeURIComponent(stateUrl);
            }
            console.log(
              `There was state of ${this.oauthService.state}, so we are sending you to: ${stateUrl}`
            );

            console.log('Navigating to stateURL -> ' + stateUrl);
            this.router.navigateByUrl(stateUrl);
          }
        })
        .catch(() => this.isDoneLoadingSubject$.next(true))
    );
  }

  login(model: LoginModel): Observable<any> {
    let url = environment.backEndUrl + ApiPaths.Login;
    return this.http.post<any>(environment.backEndUrl + ApiPaths.Login, model);
  }

  loginViaKeycloak(targetUrl?: string) {
    // Note: before version 9.1.0 of the library you needed to
    // call encodeURIComponent on the argument to the method.
    this.oauthService.initCodeFlow(targetUrl);
  }

  logOut() {
    this.oauthService.revokeTokenAndLogout();
  }

  public refresh(): Promise<TokenResponse> {
    return this.oauthService.refreshToken();
  }
  public hasValidToken() {
    return this.oauthService.hasValidAccessToken();
  }

  // These normally won't be exposed from a service like this, but
  // for debugging it makes sense.
  public get accessToken() {
    return this.oauthService.getAccessToken();
  }
  public get refreshToken() {
    return this.oauthService.getRefreshToken();
  }
  public get identityClaims() {
    return this.oauthService.getIdentityClaims();
  }
  public get idToken() {
    return this.oauthService.getIdToken();
  }
  public get logoutUrl() {
    return this.oauthService.logoutUrl;
  }

  loggedIn() {
    let token = this.getToken();

    return token && !this.jwtService.isTokenExpired(token);
  }

  saveToken(token) {
    localStorage.setItem('token', token);
  }

  getToken() {
    let identityToken = localStorage.getItem('token');
    let oauth2AccessToken = this.accessToken;
    if (identityToken) return identityToken;
    return oauth2AccessToken;
  }

  getId(): number {
    if (this.userId) return this.userId;

    let token = this.getToken();

    let decoded = this.jwtService.decodeToken(token);
    this.userId = +decoded.id;

    return this.userId;
  }
}
