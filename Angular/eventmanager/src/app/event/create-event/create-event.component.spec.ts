import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {CreateEventComponent} from './create-event.component';
import {APP_BASE_HREF, Location} from '@angular/common';
import {EventService} from '../event.service';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {Router} from '@angular/router';
import {EventServiceStub, LocationStub, RouterStub} from '../../shared/TestCommons/stubs';
import {MdDialogModule} from '@angular/material';

describe('CreateEventComponent', () => {
  let component: CreateEventComponent;
  let fixture: ComponentFixture<CreateEventComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [MdDialogModule],
      declarations: [CreateEventComponent],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'},
        {provide: EventService, useClass: EventServiceStub},
        {provide: Router, useClass: RouterStub},
        {provide: Location, useClass: LocationStub},
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(CreateEventComponent);
        component = fixture.componentInstance;
      });
  }));

  it('CreateEvent creation should succeed', () => {
    expect(component).toBeTruthy();
  });

  xit('Create event with invalid startDate should return expected error', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Bitte geben Sie ein korrektes Startdatum an!');
  });

  xit('Create event with invalid endDate should return expected error', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Bitte geben Sie ein korrektes Enddatum an!');
  });

  xit('Create event with invalid budget should return expected error', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Wenn ein Budget angegeben wird, muss dieses positiv oder null sein!');
  });

  xit('Create event without title should return expected error', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Bitte geben Sie einen Titel an!');
  });
});
