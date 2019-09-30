import {Injectable} from '@angular/core';
import {ServiceTypeModel} from '../shared/models/service-types-model';
import {Observable} from 'rxjs/Observable';
import {HttpClient} from '@angular/common/http';
import {IEventService} from '../shared/models/ievent-service';

@Injectable()
export class ServiceTypesService {
  private _http: HttpClient;

  constructor(http: HttpClient) {
    this._http = http;
  }

  getServiceTypes(): Observable<ServiceTypeModel[]> {
    return this._http.get('/api/servicetypes');
  }

  updateServiceType(serviceProvider: ServiceTypeModel): Observable<void> {
    return this._http.post(`api/servicetypes/${serviceProvider.id}`, JSON.stringify(serviceProvider), {responseType: 'text'}).map(() => {
    });
  }

  createServiceType(serviceProvider: ServiceTypeModel): Observable<void> {
    return this._http.put(`api/servicetypes`, JSON.stringify(serviceProvider), {responseType: 'text'}).map(() => {
    });
  }

  deleteServiceType(id: number): Observable<void> {
    return this._http.delete(`api/servicetypes/${id}`, {responseType: 'text'}).map(() => {
    });
  }

  getServiceType(typeId: number): Observable<ServiceTypeModel> {
    return this._http.get(`api/servicetypes/${typeId}`);
  }

  getServicesForServiceType(typeId: number): Observable<IEventService[]> {
    return this._http.get<IEventService[]>(`/api/ServiceTypes/${typeId}/serviceProvider`);
  }
}
