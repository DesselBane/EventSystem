import {LOCALE_ID, NgModule, NO_ERRORS_SCHEMA} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CreateEventComponent} from './create-event/create-event.component';
import {EventRoutingModule} from './event-routing.module';
import {FlexLayoutModule} from '@angular/flex-layout';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {EventListComponent} from './event-list/event-list.component';
import {EventService} from './event.service';
import {EventAttributeListingComponent} from './event-attribute-listing/event-attribute-listing.component';
import {UpdateEventComponent} from './update-event/update-event.component';
import {DeleteDialogComponent} from './delete-dialog/delete-dialog.component';
import {EventDetailsComponent} from './event-details/event-details.component';
import {AttendeeOverviewComponent} from './attendee-overview/attendee-overview.component';
import {PersonModule} from '../person/person.module';
import {MaterialMetaModule} from '../material-meta/material-meta.module';
import {ServiceSlotsOverviewComponent} from './service-slots-overview/service-slots-overview.component';
import {CreateServiceSlotComponent} from './create-service-slot/create-service-slot.component';
import {UpdateServiceSlotComponent} from './update-service-slot/update-service-slot.component';
import {ServiceTypesModule} from '../service-types/service-types.module';
import {ServiceSlotDetailsComponent} from './service-slot-details/service-slot-details.component';
import {EventServiceModule} from '../event-service/event-service.module';


@NgModule({
  imports: [
    CommonModule,
    EventRoutingModule,
    FlexLayoutModule,
    FormsModule,
    ReactiveFormsModule,
    PersonModule,
    MaterialMetaModule,
    ServiceTypesModule,
    EventServiceModule,
  ],
  declarations: [
    CreateEventComponent,
    EventListComponent,
    EventAttributeListingComponent,
    UpdateEventComponent,
    DeleteDialogComponent,
    EventDetailsComponent,
    AttendeeOverviewComponent,
    ServiceSlotsOverviewComponent,
    CreateServiceSlotComponent,
    UpdateServiceSlotComponent,
    ServiceSlotDetailsComponent,
  ],
  providers: [
    EventService,
    {provide: LOCALE_ID, useValue: 'de-DE'},
  ],
  schemas: [
    NO_ERRORS_SCHEMA
  ],
  bootstrap: [
    DeleteDialogComponent
  ]
})
export class EventModule {
}
