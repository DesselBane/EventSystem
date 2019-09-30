/**
 * Created by hgx on 07.12.17.
 */
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {EventServiceAgreementModel} from '../shared/models/event-service-agreement-model';
import {IEventServiceAgreementResponse} from '../shared/models/ievent-service-agreement-response';

@Injectable()
export class EventServiceAgreementService {
  private http: HttpClient;

  constructor(http: HttpClient) {
    this.http = http;
  }

  getMy(): Observable<IEventServiceAgreementResponse[]> {
    return this.http.get<IEventServiceAgreementResponse[]>(`api/service/my/agreements`);
  }

  getAllByEventId(eventId: number): Observable<IEventServiceAgreementResponse[]> {
    return this.http.get<IEventServiceAgreementResponse[]>(`api/Event/${eventId}/agreements`);
  }

  requested(eventId: number, eventSpsId: number, serviceId: number): Observable<EventServiceAgreementModel> {
    return this.http.put(`api/Event/${eventId}/sps/${eventSpsId}/request/${serviceId}`, null);
  }

  proposal(eventServiceAgreementModel: EventServiceAgreementModel): Observable<EventServiceAgreementModel> {
    return this.http.post(
      `api/Event/${eventServiceAgreementModel.eventId}/sps/${eventServiceAgreementModel.serviceSlotId}/agreement/proposal`,
      JSON.stringify(eventServiceAgreementModel))
  }

  accept(eventId: number, serviceSlotId: number): Observable<EventServiceAgreementModel> {
    return this.http.post(
      `api/Event/${eventId}/sps/${serviceSlotId}/agreement/accept`, null);
  }

  decline(eventId: number, serviceSlotId: number): Observable<EventServiceAgreementModel> {
    return this.http.post(
      `api/Event/${eventId}/sps/${serviceSlotId}/agreement/decline`, null);
  }
}
