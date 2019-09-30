import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {EventModel} from '../../shared/models/event-model';

import {EventAttributeListingModes} from './event-attribute-listing-modes';
import {Router} from '@angular/router';

@Component({
  selector: 'app-event-attribute-listing',
  templateUrl: './event-attribute-listing.component.html',
  styleUrls: ['./event-attribute-listing.component.css']
})
export class EventAttributeListingComponent implements OnInit {
  @Input() event: EventModel;
  @Input() mode: EventAttributeListingModes;
  @Output() eventOutput = new EventEmitter<EventModel>();
  @Output() deleteOutput = new EventEmitter();
  @Output() cancelOutput = new EventEmitter();

  editMode = EventAttributeListingModes.Edit;
  showMode = EventAttributeListingModes.Show;
  private router: Router;

  constructor(router: Router) {
    this.router = router;
  }

  ngOnInit() {
  }

  onSubmit() {
    this.eventOutput.emit(this.event);
  }

  onDelete() {
    this.deleteOutput.emit();
  }

  goBack() {
    this.cancelOutput.emit();
  }

  onChangeToEdit() {
    this.router.navigateByUrl('/events/updateEvent/' + this.event.id).then();
  }
}
