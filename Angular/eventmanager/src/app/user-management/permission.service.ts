import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs/Observable";
import {IPermissionResponse} from "../shared/models/ipermission-response";
import {PermissionModel} from "../shared/models/permission-model";

@Injectable()
export class PermissionService {
  private _http: HttpClient;

  constructor(http: HttpClient) {
    this._http = http;
  }

  getPermissionsForUser(userId: number): Observable<PermissionModel[]> {
    return this._http.get<IPermissionResponse[]>(`api/user/${userId}/permission`)
      .map(data => data.map(single => PermissionModel.parse(single)));
  }

  createPermissionForUser(userId: number, type: string, value: string): Observable<PermissionModel> {
    return this._http.put<IPermissionResponse>(`api/user/${userId}/permission`, JSON.stringify({
      "type": type,
      "value": value
    }))
      .map(data => PermissionModel.parse(data));
  }

  deletePermissionForUser(userId: number, permissionId: number): Observable<void> {
    return this._http.delete(`api/user/${userId}/permission/${permissionId}`, {responseType: 'text'})
      .map(() => {
      });
  }

}
