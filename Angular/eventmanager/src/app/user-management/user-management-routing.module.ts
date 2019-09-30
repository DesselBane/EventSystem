import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {AuthGuard} from '../shared/auth.guard';
import {UserOverviewComponent} from "./user-overview/user-overview.component";
import {UserDetailComponent} from "./user-detail/user-detail.component";

@NgModule({
  imports: [
    RouterModule.forChild([
      {path: '', component: UserOverviewComponent, canActivate: [AuthGuard]},
      {path: 'details/:id', component: UserDetailComponent, canActivate: [AuthGuard]}
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class UserManagementRoutingModule {
}
