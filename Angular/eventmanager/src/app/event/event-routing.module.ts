import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {CreateEventComponent} from './create-event/create-event.component';
import {AuthGuard} from '../shared/auth.guard';
import {EventListComponent} from './event-list/event-list.component';
import {UpdateEventComponent} from './update-event/update-event.component';
import {EventDetailsComponent} from './event-details/event-details.component';
import {CanDeactivateGuard} from '../shared/can-deactivate.guard';
import {CreateServiceSlotComponent} from './create-service-slot/create-service-slot.component';
import {UpdateServiceSlotComponent} from './update-service-slot/update-service-slot.component';
import {ServiceSlotDetailsComponent} from "./service-slot-details/service-slot-details.component";

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: EventListComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'updateEvent/:id',
        component: UpdateEventComponent,
        canActivate: [AuthGuard],
        canDeactivate: [CanDeactivateGuard]
      },
      {
        path: 'event/:id',
        component: EventDetailsComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'createEvent',
        component: CreateEventComponent,
        canActivate: [AuthGuard],
        canDeactivate: [CanDeactivateGuard]
      },
      {
        path: 'event/:id/createServiceSlot',
        component: CreateServiceSlotComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'updateServiceSlot/:id',
        component: UpdateServiceSlotComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'event/:id/sps/:spsId',
        component: ServiceSlotDetailsComponent,
        canActivate: [AuthGuard]
      }
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class EventRoutingModule {
}
