import {IPersonResponse} from './iperson-response';

export class PersonModel implements IPersonResponse {
  lastname: string;
  id: number;
  firstname: string;
  profilePictureUrl: string;
  profileImage: string;

  constructor(firstName: string, lastName: string, profilePictureUrl: string, id: number) {
    this.firstname = firstName;
    this.lastname = lastName;
    this.profilePictureUrl = profilePictureUrl;
    this.id = id;
  }

  static parse(data: IPersonResponse): PersonModel {
    return new PersonModel(data.firstname, data.lastname, data.profilePictureUrl, data.id);
  }
}
