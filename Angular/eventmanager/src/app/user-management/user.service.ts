import {Injectable} from '@angular/core';
import {Observable} from "rxjs/Observable";
import {UserModel} from "../shared/models/user-model";
import {IUserResponse} from "../shared/models/iuser-response";
import {HttpClient} from "@angular/common/http";

@Injectable()
export class UserService {
  private _http: HttpClient;

  constructor(http: HttpClient) {
    this._http = http;
  }

  getManageableUsers(): Observable<UserModel[]> {
    return this._http.get<IUserResponse[]>("api/user")
      .map(data => data.map(userResponse => UserModel.parse(userResponse)));
  }

  getUser(userId: number): Observable<UserModel> {
    return this._http.get<IUserResponse>(`api/user/${userId}`)
      .map(data => UserModel.parse(data));
  }
}
