import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {UserOverviewComponent} from './user-overview/user-overview.component';
import {UserManagementRoutingModule} from "./user-management-routing.module";
import {FlexLayoutModule} from "@angular/flex-layout";
import {UserService} from "./user.service";
import {UserDetailComponent} from './user-detail/user-detail.component';
import {PermissionService} from "./permission.service";
import {FormsModule} from '@angular/forms';
import {PersonModule} from "../person/person.module";
import {MaterialMetaModule} from "../material-meta/material-meta.module";

@NgModule({
  imports: [
    CommonModule,
    UserManagementRoutingModule,
    FlexLayoutModule,
    FormsModule,
    PersonModule,
    MaterialMetaModule,
  ],
  declarations: [
    UserOverviewComponent,
    UserDetailComponent
  ],
  providers: [
    UserService,
    PermissionService
  ]
})
export class UserMangementModule {
}
