import {Component, OnInit} from '@angular/core';
import {UserModel} from "../../shared/models/user-model";
import {UserService} from "../user.service";
import {PersonModel} from "../../shared/models/person-model";
import {IPersonResponse} from "../../shared/models/iperson-response";
import {PersonService} from "../../person/person.service";

@Component({
  selector: 'app-user-overview',
  templateUrl: './user-overview.component.html',
  styleUrls: ['./user-overview.component.css']
})
export class UserOverviewComponent implements OnInit {

  Users: UserModel[];
  IsLoading = false;
  private _userService: UserService;
  private _personService: PersonService;

  constructor(userService: UserService, personService: PersonService) {
    this._userService = userService;
    this._personService = personService;
  }

  ngOnInit() {
    this.reloadManageableUsers();
  }

  reloadManageableUsers() {
    this.IsLoading = true;

    this._userService.getManageableUsers()
      .finally(() => this.IsLoading = false)
      .do(data => {
        for (const user of data) {
          if (user.realPersonId != null)
            this._personService.getPerson(user.realPersonId)
              .catch(() => null)
              .subscribe(data => {
                if (data != null)
                  user.realPerson = PersonModel.parse(data as IPersonResponse);
                  this._personService.getPersonPicture(user.realPersonId).subscribe((result) => {
                    if(result){
                      user.realPerson.profileImage = 'data:image/png;base64,'+ result;
                    }else{
                      user.realPerson.profileImage = '../../../assets/img/city_nuernberg.png';
                    }
                  });
              })
        }
      })
      .subscribe(data => this.Users = data);
  }
}
