import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {PersonService} from './person.service';
import { PersonAttributeListingComponent } from './person-attribute-listing/person-attribute-listing.component';
import {MaterialMetaModule} from "../material-meta/material-meta.module";
import {FlexLayoutModule} from "@angular/flex-layout";

@NgModule({
  imports: [
    CommonModule,
    MaterialMetaModule,
    FlexLayoutModule
  ],
  providers: [
    PersonService,
  ],
  declarations: [
    PersonAttributeListingComponent
  ],
  exports:[
    PersonAttributeListingComponent
  ],

})
export class PersonModule {
}
