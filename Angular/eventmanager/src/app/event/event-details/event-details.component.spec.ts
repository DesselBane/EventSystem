import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {EventDetailsComponent} from './event-details.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {EventService} from '../event.service';
import {ActivatedRouteStub, EventServiceStub, RouterStub} from '../../shared/TestCommons/stubs';
import {ActivatedRoute, Router} from '@angular/router';

describe('EventDetailsComponent', () => {
  let component: EventDetailsComponent;
  let fixture: ComponentFixture<EventDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [EventDetailsComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {provide: EventService, useClass: EventServiceStub},
        {provide: ActivatedRoute, useClass: ActivatedRouteStub},
        {provide: Router, useClass: RouterStub}
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EventDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

