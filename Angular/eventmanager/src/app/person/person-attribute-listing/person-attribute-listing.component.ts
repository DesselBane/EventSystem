import {Component, Input, OnInit} from '@angular/core';
import {UserModel} from "../../shared/models/user-model";

@Component({
  selector: 'app-person-attribute-listing',
  templateUrl: './person-attribute-listing.component.html',
  styleUrls: ['./person-attribute-listing.component.css']
})
export class PersonAttributeListingComponent implements OnInit {

  @Input()
  user: UserModel;

  constructor() {
  }

  ngOnInit() {
  }

}
