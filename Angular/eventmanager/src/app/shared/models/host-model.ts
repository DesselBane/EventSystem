/**
 * Created by Christian on 13.06.2017.
 */
export class HostModel {
  firstname: string;
  lastname: string;
  profilePicture: string;

  constructor(firstname?: string,
              lastname?: string,
              profilePicture?: string) {
    this.firstname = firstname;
    this.lastname = lastname;
    this.profilePicture = profilePicture;
  }
}
