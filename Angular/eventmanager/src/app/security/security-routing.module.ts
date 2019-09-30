import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {ChangePasswordComponent} from './changePassword/changePassword.component';
import {LoginComponent} from './login/login.component';
import {RegisterComponent} from './register/register.component';
import {ResetPasswordComponent} from './resetPassword/resetPassword.component';


@NgModule({
  imports: [
    RouterModule.forChild([
      {path: 'changePassword', component: ChangePasswordComponent},
      {path: 'login', component: LoginComponent},
      {path: 'register', component: RegisterComponent},
      {path: 'resetPassword', component: ResetPasswordComponent},
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class SecurityRoutingModule {
}
