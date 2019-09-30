import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthenticationService} from '../../shared/authentication.service';
import {AuthenticationErrorResolver} from 'app/security/authentication-errorResolver';
import {MdSnackBar} from '@angular/material';

@Component({
  moduleId: module.id,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  username: string;
  password: string;
  loading = false;
  error = '';
  resetPasswordRoute = '/auth/resetPassword';
  returnUrl: string;
  private _snackBar: MdSnackBar;
  private _route: ActivatedRoute;
  private _router: Router;
  private _authenticationService: AuthenticationService;

  constructor(route: ActivatedRoute,
              router: Router,
              authenticationService: AuthenticationService,
              snackBar: MdSnackBar) {
    this._route = route;
    this._router = router;
    this._authenticationService = authenticationService;
    this._snackBar = snackBar;
  }

  ngOnInit() {
    // reset login status
    AuthenticationService.logout();

    // get return url from route parameters or default to '/'
    this.returnUrl = this._route.snapshot.queryParams['returnUrl'] || '/';
    if (this._route.snapshot.queryParams['reason'] === 'sessionTimeout') {
      this._snackBar.open('Du wurdes ausgeloggt, da deine Sitzung abgelaufen ist.', 'OK', {duration: 5000});
    }

  }

  login() {
    this.loading = true;
    this._authenticationService.login(this.username, this.password)
      .subscribe(() => {
        this._router.navigateByUrl(this.returnUrl);
      }, error => {
        this.error = AuthenticationErrorResolver.resolve(error);
        this.loading = false;
      });
  }
}
