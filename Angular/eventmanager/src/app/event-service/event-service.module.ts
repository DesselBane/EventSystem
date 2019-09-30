/**
 * Created by hgx on 19.11.17.
 */
import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {EventServiceService} from "./event-service.service";
import {EventServiceOverviewComponent} from "./event-service-overview/event-service-overview.component";
import {FormsModule} from "@angular/forms";
import {FlexLayoutModule} from "@angular/flex-layout";
import {EventServiceRoutingModule} from "./event-service-routing-module";
import {MaterialMetaModule} from "../material-meta/material-meta.module";
import {CreateEventServiceComponent} from "./create-event-service/create-event-service.component";
import {ServiceTypesModule} from "../service-types/service-types.module";
import {ProfileModule} from "../profile/profile.module";
import {EventServiceAgreementOverviewComponent} from "./event-service-agreement-overview/event-service-agreement-overview.component";
import {EventServiceAgreementService} from "./event-service-agreement-service.service";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    FlexLayoutModule,
    EventServiceRoutingModule,
    MaterialMetaModule,
    ServiceTypesModule,
    ProfileModule,
  ],
  providers: [
    EventServiceService,
    EventServiceAgreementService,
  ],
  declarations: [
    EventServiceOverviewComponent,
    CreateEventServiceComponent,
    EventServiceAgreementOverviewComponent,
  ]
})
export class EventServiceModule {
}
