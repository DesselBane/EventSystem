import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Params, Router} from '@angular/router';
import {AuthenticationService} from '../../shared/authentication.service';
import {AuthenticationErrorResolver} from '../authentication-errorResolver';

@Component({
  selector: 'app-change-password',
  templateUrl: './changePassword.component.html',
  styleUrls: ['./changePassword.component.css']
})
export class ChangePasswordComponent implements OnInit {
  loading = false;
  error = '';
  password: string;
  passwordRepeat: string;
  resetHash = '';

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private auth: AuthenticationService) {
  }

  ngOnInit() {
    this.activatedRoute.queryParams.subscribe((params: Params) => {
      this.resetHash = params['user'];
    });
  }

  changePassword() {
    if (this.password === this.passwordRepeat) {
      this.loading = true;

      this.auth.changePassword(this.password, this.resetHash)
        .subscribe(() => this.router.navigate(['/auth/login']), postError => {
        this.error = AuthenticationErrorResolver.resolve(postError);
        this.loading = false;
        });

    } else {
      this.error = 'Die Passwörter stimmen nicht überein!';
    }
  }
}
