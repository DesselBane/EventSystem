import {Component, OnInit} from '@angular/core';
import {EventService} from '../event.service';
import {UrlHelper} from '../../shared/url-helper';
import {EventModel} from '../../shared/models/event-model';
import {ActivatedRoute, Router} from '@angular/router';
import {EventAttributeListingModes} from '../event-attribute-listing/event-attribute-listing-modes';
import {EventErrorResolver} from '../event-error-resolver';

@Component({
  selector: 'app-event-details',
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.css']
})
export class EventDetailsComponent implements OnInit {
  event = new EventModel();
  errorMessage: string;
  mode = EventAttributeListingModes.Show;
  private urlHelper: UrlHelper;
  private eventService: EventService;
  private route: ActivatedRoute;
  private router: Router;

  constructor(eventService: EventService, route: ActivatedRoute, router: Router) {
    this.eventService = eventService;
    this.route = route;
    this.router = router;
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

}
