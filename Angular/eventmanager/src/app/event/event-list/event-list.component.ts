import {Component, OnInit} from '@angular/core';
import {EventModel} from '../../shared/models/event-model';
import {EventService} from '../event.service';

@Component({
  selector: 'app-event-overview',
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.css'],
})
export class EventListComponent implements OnInit {
  events: EventModel[];

  constructor(private eventService: EventService) {
  }

  ngOnInit() {
    this.eventService.getEvents().map(response => {
      const events: EventModel[] = [];

      for (const tempEvent of response) {
        events.push(EventModel.parse(tempEvent));
      }
      return events;
    })
      .subscribe(result => {
        this.events = result;
      });
  }
}
