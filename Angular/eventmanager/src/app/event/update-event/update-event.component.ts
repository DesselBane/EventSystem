import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {EventService} from '../event.service';
import {EventModel} from '../../shared/models/event-model';
import {MdDialog} from '@angular/material';
import {DeleteDialogComponent} from '../delete-dialog/delete-dialog.component';
import {UrlHelper} from '../../shared/url-helper';
import {EventAttributeListingModes} from '../event-attribute-listing/event-attribute-listing-modes';
import {EventErrorResolver} from '../event-error-resolver';
import {ICanComponentDeactivate} from '../../shared/ican-component-deactivate';
import {Observable} from 'rxjs/Observable';
import {SaveChangesDialogComponent} from '../../shared/save-changes-dialog/save-changes-dialog.component';
import {Location} from '@angular/common';

@Component({
  selector: 'app-update-event',
  templateUrl: './update-event.component.html',
  styleUrls: ['./update-event.component.css']
})
export class UpdateEventComponent implements OnInit, ICanComponentDeactivate {

  errorMessage = '';
  event = new EventModel();
  mode = EventAttributeListingModes.Edit;
  private eventService: EventService;
  private route: ActivatedRoute;
  private dialog: MdDialog;
  private router: Router;
  private urlHelper: UrlHelper;
  private triggerCanDeactivate = true;
  private _location: Location;

  constructor(eventService: EventService, route: ActivatedRoute, dialog: MdDialog, router: Router, location: Location) {
    this.eventService = eventService;
    this.route = route;
    this.dialog = dialog;
    this.router = router;
    this._location = location;
    this.urlHelper = new UrlHelper();
  }

  ngOnInit() {
    const id = this.urlHelper.GetUrlQueryString(this.route, 'id');

    this.eventService.getEvent(id)
      .subscribe(response => {
        this.event = EventModel.parse(response);
      }, error => {
        this.errorMessage = EventErrorResolver.resolve(error);
      });
  }

  updateEvent(event: EventModel) {
    this.eventService.updateEvent(event)
      .subscribe(data => {
        this.triggerCanDeactivate = false;
        this.router.navigate([`/events/event/${data.id}`]);
      }, error => {
        this.errorMessage = EventErrorResolver.resolve(error);
      });
  }

  openDeleteDialog() {
    this.dialog.open(DeleteDialogComponent)
      .afterClosed()
      .subscribe(result => {
        if (result === 'Ja') {
          this.deleteEvent();
        }
      });
  }

  deleteEvent() {
    let id = null;
    this.route.params.subscribe(params => {
      id = params['id'];
    });

    this.eventService.deleteEvent(id)
      .subscribe(() => {
        this.triggerCanDeactivate = false;
        this.router.navigate(['/events']);
      }, error => {
        console.log(error);
        this.errorMessage = EventErrorResolver.resolve(error)
      });
  }

  cancle() {
    this.triggerCanDeactivate = false;
    this._location.back();
  }

  canDeactivate(): Observable<boolean> {
    if (!this.triggerCanDeactivate) {
      return Observable.of(true);
    }

    return this.dialog.open(SaveChangesDialogComponent)
      .afterClosed()
      .mergeMap(data => {
        switch (data ) {
          case 'continue':
            return Observable.of(true);
          case 'saveContinue':
          return this.eventService.updateEvent(this.event)
            .map(() => true)
            .catch(() => Observable.of(false));
        default:
          return Observable.of(false);
        }
      });
  }

}
