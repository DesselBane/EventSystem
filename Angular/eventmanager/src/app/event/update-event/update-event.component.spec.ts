import {async, ComponentFixture, TestBed} from '@angular/core/testing';
import {UpdateEventComponent} from './update-event.component';
import {APP_BASE_HREF, Location} from '@angular/common';
import {EventService} from '../event.service';
import {ActivatedRouteStub, EventServiceStub, LocationStub, RouterStub} from '../../shared/TestCommons/stubs';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MdDialogModule} from '@angular/material';

describe('UpdateEventComponent', () => {
  let component: UpdateEventComponent;
  let fixture: ComponentFixture<UpdateEventComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [MdDialogModule],
      declarations: [UpdateEventComponent],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'},
        {provide: EventService, useClass: EventServiceStub},
        {provide: ActivatedRoute, useClass: ActivatedRouteStub},
        {provide: Router, useClass: RouterStub},
        {provide: Location, useClass: LocationStub}
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(UpdateEventComponent);
        component = fixture.componentInstance;
      });
  }));

  it('UpdateEvent creation should succeed', () => {
    expect(component).toBeTruthy();
  });

  xit('Goto event without permission should return expected result', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Sie haben nicht die nötige Berechtigung, um dieses Event anzusehen!');
  });

  xit('Goto not existing event should return expected result', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Das ausgewählte Event existiert nicht!');
  });

  xit('Update event without permission should return expected result', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Sie haben nicht die nötige Berechtigung, um dieses Event zu ändern!');
  });

  xit('Update not existing event should return expected result', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Das ausgewählte Event existiert nicht!');
  });

  xit('Update event with wrong startDate should return expected result', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Bitte geben Sie  ein korrektes Startdatum an!');
  });

  xit('Update event with wrong endDate should return expected result', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Bitte geben Sie ein korrektes Enddatum an!');
  });

  xit('Update event with wrong budget should return expected result', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Wenn ein Budget angegeben wird, muss dieses positiv oder null sein!');
  });

  xit('Update event without title should return expected result', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Bitte geben Sie einen Titel an!');
  });
});
