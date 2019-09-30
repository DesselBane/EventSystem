import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AuthenticationService} from './authentication.service';
import {CanDeactivateGuard} from './can-deactivate.guard';
import {SaveChangesDialogComponent} from './save-changes-dialog/save-changes-dialog.component';
import {MaterialMetaModule} from "../material-meta/material-meta.module";

@NgModule({
  imports: [
    CommonModule,
    MaterialMetaModule
  ],
  declarations: [SaveChangesDialogComponent],
  providers: [
    AuthenticationService,
    CanDeactivateGuard
  ],
  bootstrap: [SaveChangesDialogComponent]
})
export class SharedModule {
}
