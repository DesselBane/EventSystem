import {Component, OnInit} from '@angular/core';
import {EventService} from '../event.service';
import {AttendeeModel} from '../../shared/models/attendee-model';
import {ActivatedRoute} from '@angular/router';
import {UrlHelper} from '../../shared/url-helper';
import {PersonService} from '../../person/person.service';
import {PersonModel} from '../../shared/models/person-model';
import {Observable} from 'rxjs/Observable';
import {FormControl} from '@angular/forms';

@Component({
  selector: 'app-attendee-overview',
  templateUrl: './attendee-overview.component.html',
  styleUrls: ['./attendee-overview.component.css']
})
export class AttendeeOverviewComponent implements OnInit {
  private eventService: EventService;
  private attendeePersons: PersonModel[] = [];
  private urlHelper = new UrlHelper();
  private route: ActivatedRoute;
  private personService: PersonService;
  private searchControl = new FormControl();
  private filteredOptions: Observable<PersonModel[]>;
  private returnedPersons: PersonModel[];
  private eventId: number;

  constructor(eventService: EventService, personService: PersonService, route: ActivatedRoute) {
    this.eventService = eventService;
    this.personService = personService;
    this.route = route;
  }

  ngOnInit() {
    this.eventId = this.urlHelper.GetUrlQueryString(this.route, 'id');
    const attendees: AttendeeModel[] = [];

    this.eventService.getEventAttendees(this.eventId)
      .subscribe(
        (attendeeResponse) => {
          for (const attendee of attendeeResponse) {
            attendees.push(AttendeeModel.parse(attendee));
          }

          this.loadPersonsFromAttendeeRelationships(attendees);
        });

    this.filteredOptions = this.searchControl.valueChanges.do(() => {
      this.personService.searchPerson(this.searchControl.value)
        .subscribe(response => {
          this.returnedPersons = [];
          response.map(person => this.returnedPersons.push(PersonModel.parse(person)));
        });
    }).map(() => {
      return this.returnedPersons;
    });
  }

// noinspection JSMethodCanBeStatic
  displayFunction(person: PersonModel): string {
    return person ? `${person.firstname} ${person.lastname}` : '';
  }

  addPerson() {
    if (this.searchControl.value !== null) {
      this.eventService.createEventAttendee(new AttendeeModel(this.eventId, this.searchControl.value.id, 1))
        .subscribe(
          () => {
            this.attendeePersons.push(this.searchControl.value);
          });
    }
  }

  deleteSelectedAttendees(selectedAttendees: any[]) {
    selectedAttendees.map(attendee => this.eventService.deleteEventAttendee(this.eventId, attendee._element.nativeElement.id).subscribe(
      () => {
        const indexOfElementToDelete = this.attendeePersons.indexOf(attendee._element.nativeElement.id);
        this.attendeePersons.splice(indexOfElementToDelete, 1);
      }));
  }

  private loadPersonsFromAttendeeRelationships(attendees: AttendeeModel[]) {
    if (attendees.length !== 0) {
      attendees.map(attendee => this.personService.getPerson(attendee.personId).subscribe(
        (personResponse) => {
          this.attendeePersons.push(PersonModel.parse(personResponse));
        }));
    }
  }
}
