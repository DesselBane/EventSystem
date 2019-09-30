import {IUserResponse} from "./iuser-response";
import {PersonModel} from "./person-model";
import {PermissionModel} from "./permission-model";

export class UserModel implements IUserResponse {
  id: number;
  eMail: string;
  realPersonId: number;
  realPerson: PersonModel;
  permissions: PermissionModel[] = [];

  static parse(data: IUserResponse): UserModel {
    let user = new UserModel();

    user.eMail = data.eMail;
    user.id = data.id;
    user.realPersonId = data.realPersonId;

    return user;
  }
}
