import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MdAutocompleteModule,
  MdButtonModule,
  MdCardModule,
  MdCheckboxModule,
  MdDatepickerModule,
  MdDialogModule,
  MdExpansionModule,
  MdIconModule,
  MdInputModule,
  MdListModule,
  MdNativeDateModule,
  MdProgressBarModule,
  MdSelectModule,
  MdSidenavModule,
  MdSnackBarModule,
  MdToolbarModule
} from '@angular/material';

@NgModule({
  imports: [
    CommonModule,
    MdAutocompleteModule,
    ...
  ],
  exports: [
    MdAutocompleteModule,
    ...
  ]
})
export class MaterialMetaModule { }
