import {Component, OnInit} from '@angular/core';
import {EventService} from '../event.service';
import {EventModel} from '../../shared/models/event-model';
import {Router} from '@angular/router';
import {EventAttributeListingModes} from '../event-attribute-listing/event-attribute-listing-modes';
import {EventErrorResolver} from '../event-error-resolver';
import {MdDialog} from '@angular/material';
import {ICanComponentDeactivate} from '../../shared/ican-component-deactivate';
import {Observable} from 'rxjs/Observable';
import {SaveChangesDialogComponent} from '../../shared/save-changes-dialog/save-changes-dialog.component';
import {Location} from '@angular/common';

@Component({
  selector: 'app-create-event',
  templateUrl: './create-event.component.html',
  styleUrls: ['./create-event.component.css']
})
export class CreateEventComponent implements OnInit, ICanComponentDeactivate {

  errorMessage = '';
  event = new EventModel();
  mode = EventAttributeListingModes.Create;
  private _router: Router;
  private _eventService: EventService;
  private _dialog: MdDialog;
  private _triggerCanDeactivate = true;
  private _location: Location;

  constructor(eventService: EventService, router: Router, dialog: MdDialog, location: Location) {
    this._eventService = eventService;
    this._router = router;
    this._dialog = dialog;
    this._location = location;
  }

  ngOnInit() {
  }

  createEvent(event: EventModel) {
    this._eventService.createEvent(event)
      .subscribe((data) => {
        this._triggerCanDeactivate = false;
        this._router.navigate([`event/${data.id}`]);
      }, error => {
        this.errorMessage = EventErrorResolver.resolve(error);
      });
  }

  cancle() {
    this._triggerCanDeactivate = false;
    this._location.back();
  }

  canDeactivate(): Observable<boolean> {
    if (!this._triggerCanDeactivate) {

      return Observable.of(true);
    }

    return this._dialog.open(SaveChangesDialogComponent)
      .afterClosed()
      .mergeMap(data => {
        switch (data ) {
          case 'continue':
            return Observable.of(true);
          case 'saveContinue':
          return this._eventService.createEvent(this.event)
            .map(() => true)
            .catch(() => Observable.of(false));
        default:
          return Observable.of(false);
        }

      });
  }
}
