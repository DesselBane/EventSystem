import {async, ComponentFixture, TestBed} from '@angular/core/testing';
import {EventAttributeListingComponent} from './event-attribute-listing.component';
import {APP_BASE_HREF} from '@angular/common';
import {Router} from '@angular/router';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {MdDatepickerModule, MdNativeDateModule} from '@angular/material';
import {RouterStub} from '../../shared/TestCommons/stubs';

describe('EventAttributeListingComponent', () => {
  let component: EventAttributeListingComponent;
  let fixture: ComponentFixture<EventAttributeListingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [EventAttributeListingComponent],
      imports: [FormsModule, ReactiveFormsModule, MdDatepickerModule, MdNativeDateModule],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'},
        {provide: Router, useClass: RouterStub},
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(EventAttributeListingComponent);
        component = fixture.componentInstance;
      });
  }));

  it('EventDetails creation should succeed', () => {
    expect(component).toBeTruthy();
  });
});

