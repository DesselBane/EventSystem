import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {ServiceTypesOverviewComponent} from './service-types-overview.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {ServiceTypesService} from '../service-types.service';
import {ServiceTypesServiceStub} from '../../shared/TestCommons/stubs';

describe('ServiceTypesOverviewComponent', () => {
  let component: ServiceTypesOverviewComponent;
  let fixture: ComponentFixture<ServiceTypesOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ServiceTypesOverviewComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {provide: ServiceTypesService, useClass: ServiceTypesServiceStub}
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceTypesOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});
