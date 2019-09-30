import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FormControl} from '@angular/forms';
import {EventService} from '../event.service';
import {UrlHelper} from '../../shared/url-helper';
import {EventServiceService} from '../../event-service/event-service.service';
import {EventServiceModel} from '../../shared/models/event-service';
import {EventServiceAgreementService} from '../../event-service/event-service-agreement-service.service';
import {ServiceSlotModel} from '../../shared/models/service-slot-model';
import {EventErrorResolver} from '../event-error-resolver';
import {EventServiceAgreementModel} from '../../shared/models/event-service-agreement-model';
import {ServiceTypesService} from '../../service-types/service-types.service';

@Component({
  selector: 'app-create-service-slot',
  templateUrl: './service-slot-details.component.html',
  styleUrls: ['./service-slot-details.component.css']
})
export class ServiceSlotDetailsComponent implements OnInit {
  private eventServiceModelsSelectControl = new FormControl();
  private eventId: number;
  private urlHelper = new UrlHelper();
  // noinspection JSMismatchedCollectionQueryUpdate
  private eventServiceModels: EventServiceModel[];
  private router: Router;
  private eventService: EventService;
  private route: ActivatedRoute;
  private eventServiceService: EventServiceService;
  private eventServiceAgreementService: EventServiceAgreementService;
  private spsId: number;
  private currentServiceSlot: ServiceSlotModel;
  private error = null;
  private currentEventService: EventServiceModel;
  private currentAgreement: EventServiceAgreementModel = new EventServiceAgreementModel();
  private serviceTypeService: ServiceTypesService;
  private isLoaded = false;

  constructor(router: Router,
              eventService: EventService,
              route: ActivatedRoute,
              eventServiceService: EventServiceService,
              eventServiceAgreementService: EventServiceAgreementService,
              eventServiceTypeService: ServiceTypesService) {
    this.router = router;
    this.eventService = eventService;
    this.route = route;
    this.eventServiceService = eventServiceService;
    this.eventServiceAgreementService = eventServiceAgreementService;
    this.serviceTypeService = eventServiceTypeService;
  }

  ngOnInit() {
    this.eventId = this.urlHelper.GetUrlQueryString(this.route, 'id');
    this.spsId = this.urlHelper.GetUrlQueryString(this.route, 'spsId');

    this.loadCurrentServiceSlot();
    this.loadCurrentAgreement();
  }

  loadEventServices() {
    this.serviceTypeService.getServicesForServiceType(this.currentServiceSlot.typeId)
      .subscribe((response: EventServiceModel[]) => {
        this.eventServiceModels = response;
      });
  }

  addServiceAgreement() {
    if (this.eventServiceModelsSelectControl.value !== null) {
      const selectedEventServiceModel = this.eventServiceModelsSelectControl.value;

      this.eventServiceAgreementService.requested(this.eventId, this.spsId, selectedEventServiceModel.id)
        .subscribe(() => {
            this.router.navigateByUrl(`events/event/${this.eventId}`).then();
          },
          error => {
            this.error = EventErrorResolver.resolve(error);
          });
    }
  }

  deleteServiceAgreement() {
    this.eventService.deleteServiceAgreementForSlot(this.eventId, this.spsId).subscribe(
      () => {
        this.router.navigateByUrl(`events/event/${this.eventId}`).then();
      }
    );
  }

  deleteServiceSlot() {
    this.eventService.deleteServiceSlot(this.eventId, this.spsId).subscribe(
      () => {
        this.router.navigateByUrl(`events/event/${this.eventId}`).then();
      }
    );
  }

  private loadCurrentServiceSlot() {
    this.eventService.getSingleServiceSlot(this.eventId, this.spsId).subscribe(
      response => {
        this.currentServiceSlot = ServiceSlotModel.parse(response);

        this.loadEventServices();
      });
  }

  private loadCurrentAgreement() {
    this.eventService.getServiceAgreementForSlot(this.eventId, this.spsId)
      .finally(() => {
        this.isLoaded = true;
      }).subscribe(
      agreement => {
        this.currentAgreement = EventServiceAgreementModel.parse(agreement);

        this.eventServiceService.get(agreement.eventServiceModelId).subscribe(
          eventService => {
            this.currentEventService = EventServiceModel.parse(eventService);
          }
        )
      }
    );
  }
}
