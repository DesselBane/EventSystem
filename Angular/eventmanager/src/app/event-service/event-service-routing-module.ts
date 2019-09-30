import {NgModule} from "@angular/core";
import {RouterModule} from "@angular/router";
import {AuthGuard} from "../shared/auth.guard";
import {EventServiceOverviewComponent} from "./event-service-overview/event-service-overview.component";
import {CreateEventServiceComponent} from "./create-event-service/create-event-service.component";
import {EventServiceAgreementOverviewComponent} from "./event-service-agreement-overview/event-service-agreement-overview.component";

@NgModule({
  imports: [
    RouterModule.forChild([
      {path: '', component: EventServiceOverviewComponent, canActivate: [AuthGuard]},
      {path: 'createEventService', component: CreateEventServiceComponent, canActivate: [AuthGuard]},
      {path: 'eventServiceAgreements', component: EventServiceAgreementOverviewComponent, canActivate: [AuthGuard]}
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class EventServiceRoutingModule {
}
