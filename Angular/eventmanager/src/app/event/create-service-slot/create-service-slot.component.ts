import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ServiceTypeModel} from '../../shared/models/service-types-model';
import {FormControl} from '@angular/forms';
import {EventService} from '../event.service';
import {ServiceSlotModel} from '../../shared/models/service-slot-model';
import {UrlHelper} from '../../shared/url-helper';
import {ServiceTypesService} from '../../service-types/service-types.service';
import {EventErrorResolver} from '../event-error-resolver';

@Component({
  selector: 'app-create-service-slot',
  templateUrl: './create-service-slot.component.html',
  styleUrls: ['./create-service-slot.component.css']
})
export class CreateServiceSlotComponent implements OnInit {
  private router: Router;
  // noinspection JSMismatchedCollectionQueryUpdate
  private filteredOptions: ServiceTypeModel[];
  private serviceTypeFormControl = new FormControl();
  private startTimeFormControl = new FormControl();
  private endTimeFormControl = new FormControl();
  private targetBudgetFormControl = new FormControl();
  private eventService: EventService;
  private eventId: number;
  private urlHelper = new UrlHelper();
  private route: ActivatedRoute;
  private serviceTypesService: ServiceTypesService;
  private error: string;

  constructor(router: Router, eventService: EventService, route: ActivatedRoute, serviceTypesService: ServiceTypesService) {
    this.router = router;
    this.eventService = eventService;
    this.route = route;
    this.serviceTypesService = serviceTypesService;
  }

  ngOnInit() {
    this.eventId = this.urlHelper.GetUrlQueryString(this.route, 'id');
    this.loadServiceTypes();
  }

  // noinspection JSMethodCanBeStatic
  displayFunction(serviceType: ServiceTypeModel): string {
    return serviceType ? `${serviceType.name}` : '';
  }

  loadServiceTypes() {
    this.serviceTypesService.getServiceTypes()
      .subscribe((result: ServiceTypeModel[]) => {
        this.filteredOptions = result;
      });
  }

  addServiceSlot() {
    this.eventService.createServiceSlot(new ServiceSlotModel(
      1,
      this.targetBudgetFormControl.value,
      this.startTimeFormControl.value,
      this.endTimeFormControl.value,
      this.eventId,
      this.serviceTypeFormControl.value.id
    )).subscribe(() => {
      this.router.navigateByUrl(`events/event/${this.eventId}`).then();
    }, error => {
      this.error = EventErrorResolver.resolve(error);
    });
  }
}
