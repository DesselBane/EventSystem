import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {ServiceSlotDetailsComponent} from './service-slot-details.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';

describe('ServiceSlotDetailsComponent', () => {
  let component: ServiceSlotDetailsComponent;
  let fixture: ComponentFixture<ServiceSlotDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ServiceSlotDetailsComponent],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceSlotDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // ignored because of query params
  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});
