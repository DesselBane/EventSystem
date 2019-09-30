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
    MdButtonModule,
    MdCardModule,
    MdDatepickerModule,
    MdDialogModule,
    MdInputModule,
    MdListModule,
    MdNativeDateModule,
    MdProgressBarModule,
    MdSnackBarModule,
    MdIconModule,
    MdCheckboxModule,
    MdSelectModule,
    MdExpansionModule,
    MdSidenavModule,
    MdToolbarModule
  ],
  exports: [
    MdAutocompleteModule,
    MdButtonModule,
    MdCardModule,
    MdDatepickerModule,
    MdDialogModule,
    MdInputModule,
    MdListModule,
    MdNativeDateModule,
    MdProgressBarModule,
    MdSnackBarModule,
    MdIconModule,
    MdCheckboxModule,
    MdSelectModule,
    MdExpansionModule,
    MdSidenavModule,
    MdToolbarModule
  ]
})
export class MaterialMetaModule { }
