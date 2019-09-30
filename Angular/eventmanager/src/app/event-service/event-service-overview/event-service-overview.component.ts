import {Component, OnInit} from "@angular/core";
import {EventServiceService} from "../event-service.service";
import {EventServiceModel} from "../../shared/models/event-service";

@Component({
  selector: 'app-event-service-overview',
  templateUrl: './event-service-overview.component.html',
  styleUrls: ['./event-service-overview.component.css']
})
export class EventServiceOverviewComponent implements OnInit {
  eventService: EventServiceModel[];
  error: string;

  constructor(private eventServiceService: EventServiceService) {
  }

  ngOnInit() {
    this.loadEventServices();
  }

  loadEventServices() {
    this.eventServiceService.getMy()
      .subscribe((result: EventServiceModel[]) => {
        this.eventService = result;
      });
  }

}
