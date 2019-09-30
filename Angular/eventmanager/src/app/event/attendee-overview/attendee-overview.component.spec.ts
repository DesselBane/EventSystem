import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {AttendeeOverviewComponent} from './attendee-overview.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {MdAutocompleteModule} from '@angular/material';
import {EventService} from '../event.service';
import {PersonService} from 'app/person/person.service';
import {ActivatedRoute} from '@angular/router';
import {Observable} from 'rxjs/Observable';
import {EventServiceStub, PersonServiceStub} from '../../shared/TestCommons/stubs';

describe('AttendeeOverviewComponent', () => {
  let component: AttendeeOverviewComponent;
  let fixture: ComponentFixture<AttendeeOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [MdAutocompleteModule],
      declarations: [AttendeeOverviewComponent],
      providers: [
        {provide: EventService, useClass: EventServiceStub},
        {provide: PersonService, useClass: PersonServiceStub},
        {provide: ActivatedRoute, useValue: {params: Observable.of({id: 1})}},
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AttendeeOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

