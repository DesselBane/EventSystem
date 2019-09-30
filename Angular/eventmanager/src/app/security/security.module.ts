import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {ResetPasswordComponent} from './resetPassword/resetPassword.component';
import {ChangePasswordComponent} from './changePassword/changePassword.component';
import {RegisterComponent} from './register/register.component';
import {LoginComponent} from './login/login.component';
import {FlexLayoutModule} from '@angular/flex-layout';
import {SecurityRoutingModule} from './security-routing.module';
import {MaterialMetaModule} from "../material-meta/material-meta.module";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    FlexLayoutModule,
    SecurityRoutingModule,
    MaterialMetaModule
  ],
  declarations: [
    RegisterComponent,
    ChangePasswordComponent,
    ResetPasswordComponent,
    LoginComponent
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class SecurityModule {
}
