import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ProfileComponent} from './profile/profile.component';
import {FormsModule} from '@angular/forms';
import {FlexLayoutModule} from '@angular/flex-layout';
import {ProfileRoutingModule} from './profile-routing.module';
import {ImageCropperComponent} from 'ng2-img-cropper';
import {ChangeProfilePictureDialogComponent} from './profile-image-dialog/change-profile-picture-dialog.component';
import {ProfileService} from './profile.service';
import {PersonModule} from "../person/person.module";
import {MaterialMetaModule} from "../material-meta/material-meta.module";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    FlexLayoutModule,
    ProfileRoutingModule,
    PersonModule,
    MaterialMetaModule
  ],
  declarations: [
    ProfileComponent,
    ImageCropperComponent,
    ChangeProfilePictureDialogComponent
  ],
  providers: [
    ProfileService
  ],
  entryComponents: [
    ChangeProfilePictureDialogComponent
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ProfileModule {
}
