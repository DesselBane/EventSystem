import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {FlexLayoutModule} from '@angular/flex-layout';
import {ServiceProviderRoutingModule} from './service-types-routing-module';
import {ServiceTypesOverviewComponent} from './service-types-overview/service-types-overview.component';
import {ServiceTypesService} from './service-types.service';
import {CreateServiceTypeComponent} from './create-service-type/create-service-type.component';
import {MaterialMetaModule} from "../material-meta/material-meta.module";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    FlexLayoutModule,
    ServiceProviderRoutingModule,
    MaterialMetaModule
  ],
  declarations: [
    ServiceTypesOverviewComponent,
    CreateServiceTypeComponent
  ],
  providers: [
    ServiceTypesService
  ]
})
export class ServiceTypesModule {
}
