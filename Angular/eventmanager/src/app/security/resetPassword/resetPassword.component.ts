import {Component, OnInit} from '@angular/core';
import {AuthenticationService} from '../../shared/authentication.service';
import {Router} from '@angular/router';
import {AuthenticationErrorResolver} from '../authentication-errorResolver';

@Component({
  selector: 'app-reset-password',
  templateUrl: './resetPassword.component.html',
  styleUrls: ['./resetPassword.component.css']
})
export class ResetPasswordComponent implements OnInit {
  username: string;
  loading = false;
  error = '';
  loginRoute = '/auth/login';

  constructor(private auth: AuthenticationService, private router: Router) {
  }

  ngOnInit() {
  }

  request() {
    this.loading = true;
    this.auth.resetPassword(this.username).subscribe(() => {
      this.router.navigate([this.loginRoute]);
    }, error => {
      this.error = AuthenticationErrorResolver.resolve(error);
    });
  }
}
