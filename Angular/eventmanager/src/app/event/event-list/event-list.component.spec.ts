import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {EventListComponent} from './event-list.component';
import {APP_BASE_HREF} from '@angular/common';
import {EventService} from '../event.service';
import {EventServiceStub} from '../../shared/TestCommons/stubs';
import {NO_ERRORS_SCHEMA} from '@angular/core';

describe('EventListComponent', () => {
  let component: EventListComponent;
  let fixture: ComponentFixture<EventListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [EventListComponent],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'},
        {provide: EventService, useClass: EventServiceStub}
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(EventListComponent);
        component = fixture.componentInstance;
      });
  }));

  it('EventOverview creation should succeed', () => {
    expect(component).toBeTruthy();
  });
});
