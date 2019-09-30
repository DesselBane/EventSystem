import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {UserService} from '../user.service';
import {UserModel} from 'app/shared/models/user-model';
import {UrlHelper} from '../../shared/url-helper';
import {PersonModel} from '../../shared/models/person-model';
import {IPersonResponse} from '../../shared/models/iperson-response';
import {PersonErrorResolver} from '../../person/person-error-resolver';
import {UserErrorResolver} from '../user-error-resolver';
import {PermissionService} from '../permission.service';
import {PermissionErrorResolver} from '../permission-error-resolver';
import {Observable} from 'rxjs/Observable';
import '../../rxjs-extensions';
import {PermissionModel} from '../../shared/models/permission-model';
import {PersonService} from '../../person/person.service';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent implements OnInit {
  private _permissionService: PermissionService;
  private _route: ActivatedRoute;
  private _userService: UserService;
  private _personService: PersonService;
  private _User: UserModel;
  private _IsLoading = 0;
  private _ErrorMessages: string[];
  private _allPermissionsList: { key: string, value: string }[];

  constructor(route: ActivatedRoute, userService: UserService, personService: PersonService, permissionService: PermissionService) {
    this._route = route;
    this._userService = userService;
    this._personService = personService;
    this._permissionService = permissionService;
    this._allPermissionsList = PermissionModel.lookupArray;

  }

  get IsLoading(): boolean {
    return this._IsLoading > 0;
  }

  set IsLoading(value: boolean) {
    if (value) {
      this._IsLoading++;
    } else {
      this._IsLoading--;
    }
  }

  get User(): UserModel {
    return this._User;
  }

  set User(value: UserModel) {
    this._User = value;
  }

  get ErrorMessages(): string[] {
    return this._ErrorMessages;
  }

  set ErrorMessages(value: string[]) {
    this._ErrorMessages = value;
  }

  get allPermissionsList(): { key: string, value: string }[] {
    return this._allPermissionsList;
  }

  ngOnInit() {
    this.loadUser();
  }

  loadUser() {
    this.IsLoading = true;
    this.ErrorMessages = [];

    const id = new UrlHelper().GetUrlQueryString(this._route, 'id');

    this._userService.getUser(id)
      .mergeMap(data => {
        return this._personService.getPerson(data.realPersonId)
          .catch(personError => {
            this.ErrorMessages.push(PersonErrorResolver.resolve(personError));
            return Observable.of(null);
          })
          .map(personData => {
            if (personData != null) {
              data.realPerson = PersonModel.parse(personData as IPersonResponse);
            }
            this._personService.getPersonPicture(data.realPersonId).subscribe((result) => {
              data.realPerson.profileImage = 'data:image/png;base64,' + result;
            });
            return data;
          });
      })
      .mergeMap(data => {
        return this._permissionService.getPermissionsForUser(data.id)
          .catch(permissionError => {
            this.ErrorMessages.push(PermissionErrorResolver.resolve(permissionError));
            return [];
          })
          .map(permissionData => {
            data.permissions = permissionData;
            return data;
          });
      })
      .finally(() => this.IsLoading = false)
      .subscribe(data => this.User = data, UserError => {
        this.ErrorMessages.push(UserErrorResolver.resolve(UserError));
      });
  }

  addPermission(permissionId: string) {
    if (this.User == null) {
      return;
    }

    this.IsLoading = true;
    const splittedId = permissionId.split('|', 2);
    this._permissionService.createPermissionForUser(this.User.id, splittedId[0], splittedId[1])
      .finally(() => this.IsLoading = false)
      .subscribe(data => {
        this.User.permissions.push(data);
      }, error => {
        this.ErrorMessages.push(PermissionErrorResolver.resolve(error))
      });
  }

  deletePermission(permission: PermissionModel) {
    if (this.User == null) {
      return;
    }

    this.IsLoading = true;
    this._permissionService.deletePermissionForUser(this.User.id, permission.id)
      .finally(() => this.IsLoading = false)
      .subscribe(() => this.User.permissions.splice(this.User.permissions.indexOf(permission, 0), 1));

  }

}
