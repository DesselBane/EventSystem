import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {ServiceTypesOverviewComponent} from './service-types-overview/service-types-overview.component';
import {AuthGuard} from '../shared/auth.guard';
import {CreateServiceTypeComponent} from './create-service-type/create-service-type.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {path: '', component: ServiceTypesOverviewComponent, canActivate: [AuthGuard]},
      {path: 'createServiceType', component: CreateServiceTypeComponent, canActivate: [AuthGuard]}
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class ServiceProviderRoutingModule {
}
