/**
 * Created by hgx on 12.05.17.
 */
import {Injectable} from '@angular/core';
import 'rxjs/add/operator/toPromise';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {IPersonResponse} from 'app/shared/models/iperson-response';
import {PersonModel} from '../shared/models/person-model';

@Injectable()
export class ProfileService {
  private _http: HttpClient;

  constructor(http: HttpClient) {
    this._http = http;
  }

  getCurrentProfile(): Observable<PersonModel> {
    return this._http.get<IPersonResponse>('/api/person/profile/')
      .map(profile => PersonModel.parse(profile));
  }

  updateCurrentProfile(profile: PersonModel): Observable<void> {
    const tempProfile = new PersonModel(profile.firstname, profile.lastname, '', profile.id);

    return this._http.post('/api/person/profile', JSON.stringify(tempProfile), {responseType: 'text'}).map(() => {
    });
  }






}
