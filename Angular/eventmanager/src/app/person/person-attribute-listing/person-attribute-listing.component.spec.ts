import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {PersonAttributeListingComponent} from './person-attribute-listing.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';

describe('PersonAttributeListingComponent', () => {
  let component: PersonAttributeListingComponent;
  let fixture: ComponentFixture<PersonAttributeListingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PersonAttributeListingComponent],
      schemas: [NO_ERRORS_SCHEMA],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonAttributeListingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
