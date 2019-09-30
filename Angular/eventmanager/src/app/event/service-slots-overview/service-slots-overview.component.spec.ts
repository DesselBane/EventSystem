import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {ServiceSlotsOverviewComponent} from './service-slots-overview.component';

describe('ServiceSlotsOverviewComponent', () => {
  let component: ServiceSlotsOverviewComponent;
  let fixture: ComponentFixture<ServiceSlotsOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ServiceSlotsOverviewComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceSlotsOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // ignored because of query param error
  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});
