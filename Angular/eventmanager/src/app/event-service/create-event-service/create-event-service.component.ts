import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {EventServiceModel} from '../../shared/models/event-service';
import {EventServiceService} from '../event-service.service';
import {ServiceTypesService} from '../../service-types/service-types.service';
import {ServiceTypeModel} from '../../shared/models/service-types-model';
import {ProfileService} from '../../profile/profile.service';
import {PersonModel} from '../../shared/models/person-model';

@Component({
  selector: 'app-create-event-service',
  templateUrl: './create-event-service.component.html',
  styleUrls: ['./create-event-service.component.css']
})
export class CreateEventServiceComponent implements OnInit {
  eventService: EventServiceModel = new EventServiceModel();
  serviceTypes: ServiceTypeModel[];
  person: PersonModel;
  error: string;

  constructor(private eventServiceService: EventServiceService, private router: Router, private serviceTypeService: ServiceTypesService,
              private profileService: ProfileService) {
  }

  ngOnInit() {
    this.loadAll();
    this.loadPerson();
  }

  create() {
    this.eventServiceService.create(this.eventService)
      .subscribe(() => {
        this.router.navigate([`provideEventService`]);
      }, (error) => {
        // this.error = ServiceTypesErrorResolver.resolve(error);
      });
  }

  loadAll() {
    this.serviceTypeService.getServiceTypes()
      .subscribe((result: ServiceTypeModel[]) => {
        this.serviceTypes = result;
      });
  }

  loadPerson() {
    this.profileService.getCurrentProfile()
      .subscribe((result: PersonModel) => {
        this.person = result;
        this.eventService.personId = result.id;
      })
  }
}
