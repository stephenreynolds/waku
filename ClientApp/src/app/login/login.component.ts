import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../shared/user.service';

@Component({
  selector: 'app-login',
  templateUrl: 'login.component.html'
})
export class LoginComponent {

  public creds = {
    username: '',
    password: ''
  };

  public errorMessage: string = '';

  constructor(private user: UserService, private router: Router) {}

  public onLogin() {
    this.user.login(this.creds).subscribe(success => {
      if (success) {
        this.router.navigate(['']);
      }
    }, err => this.errorMessage = 'Failed to login');
  }
}
