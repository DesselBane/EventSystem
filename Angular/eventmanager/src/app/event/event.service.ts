import {Injectable} from '@angular/core';
import {ServiceSlotModel} from '../shared/models/service-slot-model';
import {AttendeeModel} from '../shared/models/attendee-model';
import {LocationModel} from '../shared/models/location-model';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {IEventResponse} from '../shared/models/ievent-response';
import {IAttendeeResponse} from '../shared/models/iattendee-response';
import {IServiceSlotResponse} from '../shared/models/iservice-slot-response';
import {EventServiceAgreementModel} from '../shared/models/event-service-agreement-model';

@Injectable()
export class EventService {
  private http: HttpClient;

  constructor(http: HttpClient) {
    this.http = http;
  }

  getEvents(): Observable<IEventResponse[]> {
    return this.http.get<IEventResponse[]>('/api/Event');
  }

  createEvent(event: IEventResponse): Observable<IEventResponse> {
    return this.http.put<IEventResponse>('api/event', JSON.stringify(event));
  }

  deleteEvent(id: number): Observable<void> {
    return this.http.delete(`api/event/${id}`, {responseType: 'text'})
      .map(() => {
      });
  }

  getEvent(id: Number): Observable<IEventResponse> {
    return this.http.get<IEventResponse>(`/api/event/${id}`);
  }

  updateEvent(event: IEventResponse): Observable<IEventResponse> {
    return this.http.post<IEventResponse>(`api/event/${event.id}`, JSON.stringify(event));
  }

  updateEventHost(eventId: number, newHostId: number): Observable<void> {
    return this.http.post(`api/event/${eventId}/updatehost/${newHostId}`, '', {responseType: 'text'}).map(() => {
    });
  }

  getAllServicSlotsForEvent(eventId: number): Observable<IServiceSlotResponse[]> {
    return this.http.get<IServiceSlotResponse[]>(`api/event/${eventId}/sps`);
  }

  createServiceSlot(serviceProviderSlot: ServiceSlotModel): Observable<ServiceSlotModel> {
    return this.http.put<ServiceSlotModel>(`api/event/${serviceProviderSlot.eventId}/sps`, JSON.stringify(serviceProviderSlot));
  }

  deleteServiceSlot(eventId: number, id: number): Observable<void> {
    return this.http.delete(`api/event/${eventId}/sps/${id}`, {responseType: 'text'}).map(() => {
    });
  }

  getSingleServiceSlot(eventId: number, id: number): Observable<ServiceSlotModel> {
    return this.http.get<ServiceSlotModel>(`api/event/${eventId}/sps/${id}`);
  }

  updateServiceSlot(serviceProviderSlot: ServiceSlotModel): Observable<ServiceSlotModel> {
    return this.http.post<ServiceSlotModel>(`api/event/${serviceProviderSlot.eventId}/sps/${serviceProviderSlot.id}`,
      JSON.stringify(serviceProviderSlot));
  }

  deleteEventAttendee(eventId: number, attendeeId: number): Observable<void> {
    return this.http.delete(`api/event/${eventId}/attendees/${attendeeId}`, {responseType: 'text'}).map(() => {
    });
  }

  getEventAttendee(eventId: number, attendeeId: number): Observable<AttendeeModel> {
    return this.http.get<AttendeeModel>(`api/event/${eventId}/attendees/${attendeeId}`);
  }

  updateEventAttendee(attendee: AttendeeModel): Observable<AttendeeModel> {
    return this.http.post<AttendeeModel>(`api/event/${attendee.eventId}/attendees/${attendee.personId}`, JSON.stringify(attendee));
  }

  createEventAttendee(attendee: AttendeeModel): Observable<IAttendeeResponse> {
    return this.http.put<IAttendeeResponse>(`api/event/${attendee.eventId}/attendees/${attendee.personId}`, JSON.stringify(attendee));
  }

  getEventAttendees(eventId: number): Observable<IAttendeeResponse[]> {
    return this.http.get<IAttendeeResponse[]>(`api/event/${eventId}/attendees`);
  }

  getEventLocation(eventId: number): Observable<LocationModel> {
    return this.http.get<LocationModel>(`api/event/${eventId}/location`);
  }

  updateEventLocation(eventId: number, location: LocationModel): Observable<LocationModel> {
    return this.http.post<LocationModel>(`api/event/${eventId}/location`, JSON.stringify(location));
  }

  getServiceAgreementForSlot(eventId: number, spsId: number): Observable<EventServiceAgreementModel> {
    return this.http.get<EventServiceAgreementModel>(`api/event/${eventId}/sps/${spsId}/agreement`);
  }

  deleteServiceAgreementForSlot(eventId: number, spsId: number): Observable<void> {
    return this.http.delete(`api/event/${eventId}/sps/${spsId}/agreement`, {responseType: 'text'}).map(() => {
    });
  }
}
