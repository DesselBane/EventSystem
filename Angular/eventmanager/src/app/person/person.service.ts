import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {IPersonResponse} from '../shared/models/iperson-response';

@Injectable()
export class PersonService {
  private http: HttpClient;

  constructor(http: HttpClient) {
    this.http = http;
  }

  getPerson(personId: number): Observable<IPersonResponse> {
    return this.http.get<IPersonResponse>(`api/person/${personId}`);
  }

  searchPerson(searchTerm: string): Observable<IPersonResponse[]> {
    const params = new HttpParams().set('term', searchTerm);

    return this.http.get<IPersonResponse[]>(`api/person/search/`, {params: params});
  }

  getPersonPicture(personId: number): Observable<String> {
    return this.http.get<String>('api/person/'+personId+'/picture');
  }

  updatePersonPicture(picture: string): Observable<void> {
    return this.http.post('/api/person/profile/picture', JSON.stringify({picture: picture}), {responseType: 'text'}).map(() => {
    });
  }
}
