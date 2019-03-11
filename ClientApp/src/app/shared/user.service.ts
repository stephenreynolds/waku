import { Injectable, Inject } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Injectable()
export class UserService {

  constructor(private http: HttpClient, private router: Router) {

  }

  public get loginRequired(): boolean {
    const jwt = localStorage.getItem('jwt');
    const jwtexp = localStorage.getItem('jwtexp');
    if (jwt && jwtexp) {
      return (
        jwt.length === 0 ||
        new Date(jwtexp) < new Date()
      );
    }
    else {
      return true;
    }
  }

  public login(creds): Observable<boolean> {
    return this.http.post('api/account/createtoken', creds)
      .pipe(
        map((data: any) => {
          localStorage.setItem('jwt', data.token);
          localStorage.setItem('jwtexp', data.expiration);
          return true;
        })
      );
  }

  public logout() {
    localStorage.removeItem('jwt');
    localStorage.removeItem('jwtexp');
    this.router.navigate(['']);
  }
}
