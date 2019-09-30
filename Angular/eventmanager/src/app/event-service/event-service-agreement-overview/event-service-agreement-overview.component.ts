/**
 * Created by hgx on 07.12.17.
 */
import {Component, OnInit} from '@angular/core';
import {EventServiceAgreementService} from '../event-service-agreement-service.service';
import {EventServiceAgreementModel} from '../../shared/models/event-service-agreement-model';

@Component({
  selector: 'app-event-service-agreement-overview',
  templateUrl: './event-service-agreement-overview.component.html',
  styleUrls: ['./event-service-agreement-overview.component.css']
})
export class EventServiceAgreementOverviewComponent implements OnInit {
  eventServiceAgreement: EventServiceAgreementModel[] = [];
  error: string;

  constructor(private eventServiceAgreementService: EventServiceAgreementService) {
  }

  ngOnInit() {
    this.loadEventServices();
  }

  loadEventServices() {
    this.eventServiceAgreementService.getMy()
      .subscribe((result) => {
        for (const agreement of result) {
          this.eventServiceAgreement.push(EventServiceAgreementModel.parse(agreement))
        }
      });
  }

  // noinspection JSMethodCanBeStatic
  getStyle(state: number) {
    switch (state) {
      case 0:
        return 'requested';
      case 1:
        return 'proposal';
      case 2:
        return 'accepted';
      case 3:
        return 'declined';
    }
  }

  proposal(agreement: EventServiceAgreementModel) {
    this.eventServiceAgreementService.proposal(agreement)
      .subscribe(() => {
        agreement.state = 1;
      }, (error) => {
        this.error = error;
      });
  }

  decline(agreement: EventServiceAgreementModel) {
    this.eventServiceAgreementService.decline(agreement.eventId, agreement.serviceSlotId)
      .subscribe(() => {
        agreement.state = 3;
      }, (error) => {
        this.error = error;
      });
  }
}
