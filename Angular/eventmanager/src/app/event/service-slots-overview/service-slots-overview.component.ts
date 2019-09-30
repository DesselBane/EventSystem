import {Component, OnInit} from '@angular/core';
import {UrlHelper} from '../../shared/url-helper';
import {ActivatedRoute} from '@angular/router';
import {EventService} from '../event.service';
import {ServiceSlotModel} from '../../shared/models/service-slot-model';
import {ServiceTypesService} from '../../service-types/service-types.service';
import {EventServiceAgreementService} from '../../event-service/event-service-agreement-service.service';
import {EventErrorResolver} from '../event-error-resolver';

@Component({
  selector: 'app-service-slots-overview',
  templateUrl: './service-slots-overview.component.html',
  styleUrls: ['./service-slots-overview.component.css']
})
export class ServiceSlotsOverviewComponent implements OnInit {
  // noinspection JSMismatchedCollectionQueryUpdate
  private serviceSlotsWithType: FullSizedServiceSlot[] = [];
  // noinspection JSMismatchedCollectionQueryUpdate
  private serviceSlots: ServiceSlotModel[] = [];
  private urlHelper = new UrlHelper();
  private eventId: number;
  private route: ActivatedRoute;
  private eventService: EventService;
  private serviceTypesService: ServiceTypesService;
  private serviceAgreementService: EventServiceAgreementService;
  private error = '';

  constructor(route: ActivatedRoute,
              eventService: EventService,
              serviceTypeService: ServiceTypesService,
              serviceAgreementService: EventServiceAgreementService) {
    this.route = route;
    this.eventService = eventService;
    this.serviceTypesService = serviceTypeService;
    this.serviceAgreementService = serviceAgreementService;
  }

  ngOnInit() {
    this.eventId = this.urlHelper.GetUrlQueryString(this.route, 'id');

    this.loadServiceSlots();
  }

  // noinspection JSMethodCanBeStatic
  translateState(state: number) {
    switch (state) {
      case 0:
        return 'Angefragt';
      case 1:
        return 'Vorgemerkt';
      case 2:
        return 'Akzeptiert';
      case 3:
        return 'Abgelehnt';
      case 4:
        return 'Noch keinen Dienstleister angefragt';
    }
  }

  accept(agreement: FullSizedServiceSlot) {
    this.serviceAgreementService.accept(agreement.eventId, agreement.id)
      .subscribe(() => {
        agreement.state = 2;
      }, error => {
      });
  }

  decline(agreement: FullSizedServiceSlot) {
    this.serviceAgreementService.decline(agreement.eventId, agreement.id)
      .subscribe(() => {
        agreement.state = 3;
      }, error => {
      });
  }

  private loadServiceSlots() {
    this.eventService.getAllServicSlotsForEvent(this.eventId).subscribe(
      serviceSlots => {
        serviceSlots.map(serviceSlot => {
          this.serviceSlots.push(ServiceSlotModel.parse(serviceSlot));
        });

        this.getServiceTypesForServiceSlots();
      }
    );
  }

  private getServiceAgreementsForServiceSlots() {
    this.serviceSlotsWithType.map(serviceSlotWithType => {
      this.eventService.getServiceAgreementForSlot(this.eventId, serviceSlotWithType.id)
        .subscribe(serviceAgreement => {
          serviceSlotWithType.state = serviceAgreement.state;
        }, error => {
          let tempError = EventErrorResolver.resolve(error);

          if (tempError === 'No Agreement') {
            serviceSlotWithType.state = 4;
            tempError = '';
          }

          this.error = tempError;
        });
    })
  }

  private getServiceTypesForServiceSlots() {
    this.serviceSlots.map(serviceSlot => {
      let serviceTypeName = '';
      this.serviceTypesService.getServiceType(serviceSlot.typeId).subscribe(
        serviceType => {
          serviceTypeName = serviceType.name;

          this.serviceSlotsWithType.push(
            new FullSizedServiceSlot(
              serviceSlot.id,
              serviceSlot.budgetTarget,
              serviceSlot.start,
              serviceSlot.end,
              serviceSlot.eventId,
              serviceTypeName
            ));

          this.getServiceAgreementsForServiceSlots();
        });
    });
  }
}

class FullSizedServiceSlot {
  budgetTarget: number;
  start: Date;
  eventId: number;
  type: string;
  id: number;
  end: Date;
  state: number;

  constructor(id?: number,
              budgetTarget?: number,
              start?: Date,
              end?: Date,
              eventId?: number,
              type?: string,
              state?: number) {
    this.id = id;
    this.budgetTarget = budgetTarget;
    this.start = start;
    this.end = end;
    this.eventId = eventId;
    this.type = type;
    this.state = state;
  }
}
