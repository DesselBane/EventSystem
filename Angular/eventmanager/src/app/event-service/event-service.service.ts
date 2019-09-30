/**
 * Created by hgx on 19.11.17.
 */
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {IEventService} from '../shared/models/ievent-service';
import {IEventServiceLocation} from '../shared/models/ievent-service-location';
import {IEventServiceAttributes} from '../shared/models/ievent-service-attributes';
import {EventServiceModel} from '../shared/models/event-service';

@Injectable()
export class EventServiceService {
  private http: HttpClient;

  constructor(http: HttpClient) {
    this.http = http;
  }

  getLocation(serviceId: number): Observable<IEventServiceLocation> {
    return this.http.get<IEventServiceLocation>(`api/service/${serviceId}`);

  }

  getAttributes(serviceId: number): Observable<IEventServiceAttributes> {
    return this.http.get<IEventServiceAttributes>(`api/service/${serviceId}/attributes`);

  }

  get(serviceId: number): Observable<IEventService> {
    return this.http.get<IEventService>(`api/service/${serviceId}`);
  }

  getAll(): Observable<IEventService[]> {
    return this.http.get<IEventService[]>(`api/service`);
  }

  getMy(): Observable<IEventService[]> {
    return this.http.get<IEventService[]>(`api/service/my`);
  }

  update(eventService: EventServiceModel): Observable<void> {
    return this.http.post(`api/service/${eventService.id}`, JSON.stringify(eventService), {responseType: 'text'}).map(() => {});
  }

  create(eventService: EventServiceModel): Observable<void> {
    return this.http.put(`api/service`, JSON.stringify(eventService), {responseType: 'text'}).map(() => {
    });
  }

  delete(id: number): Observable<void> {
    return this.http.delete(`api/service/${id}`, {responseType: 'text'}).map(() => {});
  }
}
